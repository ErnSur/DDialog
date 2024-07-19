namespace Doublsb.Dialog
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using TMPro;
    using UnityEngine;

    internal class BasicPrinter : MonoBehaviour, IPrinter
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


        public async UniTask Print(string text, CancellationToken cancellationToken)
        {
            for (int i = 0; i < text.Length; i++)
            {
                var character = text[i];
                Text += character;
                TextPrinted?.Invoke();

                if (!cancellationToken.IsCancellationRequested && Delay != 0)
                    await UniTask.WaitForSeconds(Delay, false, PlayerLoopTiming.Update);
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
}