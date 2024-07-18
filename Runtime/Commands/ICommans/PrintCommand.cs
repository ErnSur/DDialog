namespace Doublsb.Dialog
{
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class PrintCommand : ICommand
    {
        private readonly IPrinter _printer;
        private readonly string _text;

        public PrintCommand(IPrinter printer, string text)
        {
            _printer = printer;
            _text = text;
        }

        public async UniTask Begin(CancellationToken cancellationToken)
        {
            for (int i = 0; i < _text.Length; i++)
            {
                var character = _text[i];
                _printer.Text += character;
                //CharacterPrinted?.Invoke(character);
                if (!cancellationToken.IsCancellationRequested && _printer.Delay != 0)
                    await UniTask.WaitForSeconds(_printer.Delay, false, PlayerLoopTiming.Update);
            }
        }
    }
}