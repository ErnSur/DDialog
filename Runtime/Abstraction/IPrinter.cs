namespace QuickEye.PeeDialog
{
    using System;
    using System.Text;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public interface IPrinter
    {
        /// <summary>
        /// Invoked when a text segment is printed, like a single character or a word. This is useful for sound effects.
        /// </summary>
        public event Action TextSegmentPrinted;

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
        
        /// <summary>
        /// Show or hide the printer
        /// </summary>
        /// <param name="active"></param>
        public void SetActive(bool active);
    }
}