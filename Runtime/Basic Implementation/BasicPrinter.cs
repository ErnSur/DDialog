namespace QuickEye.PeeDialog
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    /// <summary>
    /// Basic implementation of <see cref="IPrinter"/> using meant to be used with TextMeshPro or UI Toolkit.
    /// </summary>
    public abstract class BasicPrinter : IPrinter
    {
        public event Action TextSegmentPrinted;

        [field: SerializeField]
        public float Delay { get; set; } = 0.02f;

        [SerializeField]
        private Color textColor = Color.white;

        [SerializeField]
        private FontSize textSize;

        public FontSize TextSize
        {
            get => textSize;
            set
            {
                textSize = value;
                Text += $"<size={textSize.ToString()}>";
            }
        }

        public Color TextColor
        {
            get => textColor;
            set
            {
                textColor = value;
                Text += $"<color=#{ColorUtility.ToHtmlStringRGBA(textColor)}>";
            }
        }

        public abstract string Text { get; set; }

        public static CancellationTokenSource CreateSkipCts(CancellationToken cancellationToken,
                                                            [CallerMemberName] string callerMemberName = "")
        {
            var source = new CancellationTokenSource();
            UniTask.WaitUntil(() =>
                              {
                                  if (Input.GetMouseButtonDown(0) || Input.GetKey(KeyCode.Space))
                                  {
                                      Debug.Log($"Skipped: {callerMemberName}");
                                      source.Cancel();
                                      source.Dispose();
                                      return true;
                                  }

                                  return false;
                              },
                              cancellationToken: cancellationToken)
                   .Forget();

            return CancellationTokenSource.CreateLinkedTokenSource(source.Token, cancellationToken);
        }

        public async UniTask Print(string text, CancellationToken cancellationToken)
        {
            using var skipCts = CreateSkipCts(cancellationToken);
            for (int i = 0; i < text.Length; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var character = text[i];
                Text += character;
                try
                {
                    TextSegmentPrinted?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to invoke TextPrinted event: {e}");
                }

                if (skipCts.IsCancellationRequested || Delay <= 0)
                    continue;

                try
                {
                    await UniTask.WaitForSeconds(Delay, cancellationToken: skipCts.Token);
                }
                catch (Exception e)
                {
                    if (e is not OperationCanceledException)
                        Debug.LogError(e);
                }
            }
        }

        public void Reset()
        {
            TextSize = 60;
            TextColor = Color.white;
            Delay = 0.02f;
            Text = "";
        }

        public abstract void SetActive(bool active);
    }
}