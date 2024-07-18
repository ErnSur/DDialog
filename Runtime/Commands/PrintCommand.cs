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
            for (int i = 0; i < _text.Length; i++)
            {
                var character = _text[i];
                _printer.Text.Append(character);
                _printer.Print();
                if (!cancellationToken.IsCancellationRequested && _printer.Delay != 0)
                    await UniTask.WaitForSeconds(_printer.Delay, false, PlayerLoopTiming.Update);
            }
        }
    }
}