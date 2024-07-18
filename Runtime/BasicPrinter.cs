namespace Doublsb.Dialog
{
    using System;
    using System.Text;
    using TMPro;
    using UnityEngine;

    internal class BasicPrinter : MonoBehaviour, IPrinter
    {
        public event Action TextPrinted;
        [SerializeField]
        private GameObject textWindow;
        
        [SerializeField]
        private TMP_Text textComponent;

        [SerializeField]
        private Color textColor;

        [SerializeField]
        private FontSize textSize;

        public float Delay { get; set; } = 0.02f;
        
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

        public StringBuilder Text { get; set; } = new StringBuilder();

        public void Print()
        {
            textComponent.text = Text.ToString();
            TextPrinted?.Invoke();
        }

        public void Reset()
        {
            TextSize = 60;
            TextColor = Color.white;
            Delay = 0.02f;
            Text.Clear();
            textComponent.text = "";
        }

        public void SetActive(bool active)
        {
            textWindow.SetActive(active);
        }
    }
}