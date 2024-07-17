namespace Doublsb.Dialog
{
    using UnityEngine;

    public interface IPrinterSettings
    {
        public FontSize TextSize { get; set; }
        public Color TextColor { get; set; }
        public string Text { get; set; }
    }
}