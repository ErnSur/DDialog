namespace Doublsb.Dialog
{
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public class PrintCommand : Command, ICommand
    {
        private readonly IPrinter _printer;
        private readonly string _text;

        public PrintCommand(IPrinter printer, string text)
        {
            _printer = printer;
            _text = text;
        }
        
        async UniTask ICommand.Begin(CancellationToken cancellationToken)
        {
            await _printer.Print(_text, cancellationToken);
        }
    }
}