namespace Doublsb.Dialog
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using TMPro;
    using UnityEngine;

    public class BasicPrinter : MonoBehaviour, IPrinter
    {
        public event Action TextPrinted;

        [SerializeField]
        private GameObject textWindow;

        [SerializeField]
        private TMP_Text textComponent;

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
                textComponent.text += $"<size={textSize.ToString()}>";
            }
        }

        public Color TextColor
        {
            get => textColor;
            set
            {
                textColor = value;
                textComponent.text += $"<color=#{ColorUtility.ToHtmlStringRGBA(textColor)}>";
            }
        }

        public string Text
        {
            get => textComponent.text;
            set => textComponent.text = value;
        }

        public static CancellationTokenSource CreateSkipCts(CancellationToken cancellationToken, [CallerMemberName]string callerMemberName = "")
        {
            var source = new CancellationTokenSource();
            UniTask.WaitUntil(() =>
            {
                if (!Input.GetMouseButtonDown(0))
                    return false;
                Debug.Log($"Skipped: {callerMemberName}");
                source.Cancel();
                source.Dispose();
                return true;
            }, cancellationToken: cancellationToken).Forget();
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
                    TextPrinted?.Invoke();
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

        public void SetActive(bool active)
        {
            textWindow.SetActive(active);
        }
    }

    public class BasicPrinterCommandHandler : MonoBehaviour
    {
        private void Awake()
        {
            var printer = GetComponent<IPrinter>();
            var commandRunner = GetComponent<ICommandRunnerProvider>();
        }
    }
}