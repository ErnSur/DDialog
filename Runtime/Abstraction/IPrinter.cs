namespace QuickEye.PeeDialog
{
    using System.Text;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public interface IPrinter
    {
        public FontSize TextSize { get; set; }
        public Color TextColor { get; set; }
        public string Text { get; set; }
        public float Delay { get; set; }

        /// <summary>
        /// Prints Text to the screen
        /// </summary>
        UniTask Print(string text, CancellationToken cancellationToken);
        
        /// <summary>
        /// Reset state to default values
        /// </summary>
        public void Reset();
        
        public void SetActive(bool active);
    }
}