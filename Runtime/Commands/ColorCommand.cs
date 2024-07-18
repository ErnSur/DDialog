namespace Doublsb.Dialog
{
    using UnityEngine;

    public class ColorCommand : TempValueChangeCommand<Color>
    {
        private readonly IPrinter _printer;

        protected override Color Value
        {
            get => _printer.TextColor;
            set => _printer.TextColor = value;
        }

        public ColorCommand(IPrinter printer, string color)
        {
            NewValue = ColorUtility.TryParseHtmlString(color, out var c) ? c : Color.white;
            _printer = printer;
        }
    }
}