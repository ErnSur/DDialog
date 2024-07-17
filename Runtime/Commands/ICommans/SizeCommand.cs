namespace Doublsb.Dialog
{
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public class SizeCommand : ICommand
    {
        private readonly FontSize _newFontSize;
        private readonly IPrinterSettings _printerSettings;
        private FontSize _previousFontSize;

        public SizeCommand(string arg1, IPrinterSettings printerSettings)
        {
            _newFontSize = FontSize.ParseString(arg1);
            _printerSettings = printerSettings;
        }

        public UniTask Begin(DialogCommandSet dialogCommandSet, CancellationToken fastForwardToken,
            CancellationToken cancellationToken)
        {
            _previousFontSize = _printerSettings.TextSize;
            _printerSettings.TextSize = _newFontSize;
            return UniTask.CompletedTask;
        }

        public UniTask End(DialogCommandSet dialogCommandSet, CancellationToken fastForwardToken,
            CancellationToken cancellationToken)
        {
            _printerSettings.TextSize = _previousFontSize;
            return UniTask.CompletedTask;
        }
    }
}