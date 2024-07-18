namespace Doublsb.Dialog
{
    using UnityEngine;

    public interface IPrinter
    {
        public FontSize TextSize { get; set; }
        public Color TextColor { get; set; }
        public string Text { get; set; }
        public float Delay { get; set; }

        /// <summary>
        /// Reset state to default values
        /// </summary>
        public void Reset();
        
        public void SetActive(bool active);
    }
}