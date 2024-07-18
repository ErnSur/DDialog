namespace Doublsb.Dialog
{
    using System.Text;
    using UnityEngine;

    public interface IPrinter
    {
        public FontSize TextSize { get; set; }
        public Color TextColor { get; set; }
        public StringBuilder Text { get; set; }
        public float Delay { get; set; }
        
        /// <summary>
        /// Prints Text to the screen
        /// </summary>
        public void Print();
        
        /// <summary>
        /// Reset state to default values
        /// </summary>
        public void Reset();
        
        public void SetActive(bool active);
    }
}