namespace QuickEye.PeeDialog
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class PrinterTmpComponent : MonoBehaviour, IPrinter
    {
        [SerializeField]
        private PrinterTmp Printer;

        public event Action TextSegmentPrinted
        {
            add => Printer.TextSegmentPrinted += value;
            remove => Printer.TextSegmentPrinted -= value;
        }

        public FontSize TextSize
        {
            get => Printer.TextSize;
            set => Printer.TextSize = value;
        }

        public Color TextColor
        {
            get => Printer.TextColor;
            set => Printer.TextColor = value;
        }

        public string Text
        {
            get => Printer.Text;
            set => Printer.Text = value;
        }

        public float Delay
        {
            get => Printer.Delay;
            set => Printer.Delay = value;
        }

        public UniTask Print(string text, CancellationToken cancellationToken)
        {
            return Printer.Print(text, cancellationToken);
        }

        public void Reset()
        {
            Printer.Reset();
        }

        public void SetActive(bool active)
        {
            Printer.SetActive(active);
        }
    }
}