namespace Doublsb.Dialog
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public class FuncCommand : Command, ICommand
    {
        private readonly Func<CancellationToken, UniTask> _func;
        public FuncCommand(Func<CancellationToken, UniTask> func) => _func = func;
        public FuncCommand(Action action) => _func = _ => { action(); return UniTask.CompletedTask; };
        async UniTask ICommand.Begin(CancellationToken cancellationToken) => await _func(cancellationToken);
    }
}