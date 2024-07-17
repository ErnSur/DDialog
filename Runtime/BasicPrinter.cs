namespace Doublsb.Dialog
{
    using UnityEngine;
    using UnityEngine.UI;

    internal class BasicPrinter : MonoBehaviour, IPrinter
    {
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

        [SerializeField]
        private GameObject textWindow;
        
        [SerializeField]
        private Text textComponent;

        [SerializeField]
        private Color textColor;

        [SerializeField]
        private FontSize textSize;

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