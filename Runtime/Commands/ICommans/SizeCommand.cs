namespace Doublsb.Dialog
{
    public class SizeCommand : TempValueChangeCommand<FontSize>
    {
        private readonly IPrinter _printer;

        protected override FontSize Value
        {
            get => _printer.TextSize;
            set => _printer.TextSize = value;
        }

        public SizeCommand(IPrinter printer, string fontSize)
        {
            NewValue = FontSize.ParseString(fontSize);
            _printer = printer;
        }
    }
}