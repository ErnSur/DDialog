namespace Tests
{
    using Doublsb.Dialog;
    using UnityEngine;

    internal class TestPrinter : IPrinter
    {
        private FontSize _textSize;
        private Color _textColor;

        public FontSize TextSize
        {
            get => _textSize;
            set
            {
                _textSize = value;
                Text += $"<size={_textSize.ToString()}>";
            }
        }

        public Color TextColor
        {
            get => _textColor;
            set
            {
                _textColor = value;
                Text += $"<color=#{ColorUtility.ToHtmlStringRGBA(_textColor)}>";
            }
        }
        public string Text { get; set; }
        public float Delay { get; set; }
        
        public void SetActive(bool active)
        {
            throw new System.NotImplementedException();
        }
    }
}