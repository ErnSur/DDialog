namespace QuickEye.PeeDialog
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class PrinterUitkComponent : MonoBehaviour, IPrinter
    {
        [SerializeField]
        private UIDocument _uiDocument;

        [SerializeField]
        private PrinterUitk _printer = new PrinterUitk();

        private void OnEnable()
        {
            _printer.TextWindow = _uiDocument.rootVisualElement?.Q<VisualElement>("printer-window");
            _printer.TextElement = _uiDocument.rootVisualElement?.Q<TextElement>("printer-text");
        }

        public event Action TextSegmentPrinted
        {
            add => _printer.TextSegmentPrinted += value;
            remove => _printer.TextSegmentPrinted -= value;
        }

        public FontSize TextSize
        {
            get => _printer.TextSize;
            set => _printer.TextSize = value;
        }

        public Color TextColor
        {
            get => _printer.TextColor;
            set => _printer.TextColor = value;
        }

        public string Text
        {
            get => _printer.Text;
            set => _printer.Text = value;
        }

        public float Delay
        {
            get => _printer.Delay;
            set => _printer.Delay = value;
        }

        public async Task Print(string text, CancellationToken cancellationToken)
        {
            using var linkedCts =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationToken);

            await _printer.Print(text, linkedCts.Token);
        }

        public void Reset()
        {
            _printer.Reset();
        }

        public void SetActive(bool active)
        {
            _printer.SetActive(active);
        }
    }
}