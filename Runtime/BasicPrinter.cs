namespace Doublsb.Dialog
{
    using TMPro;
    using UnityEngine;

    internal class BasicPrinter : MonoBehaviour, IPrinter
    {
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

        public string Text
        {
            get => textComponent.text;
            set => textComponent.text = value;
        }

        public void SetActive(bool active)
        {
            textWindow.SetActive(active);
        }
    }
}