namespace Doublsb.Dialog
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public abstract class Command : ICommand
    {
        public ICommand Root { get; set; }
        public ICommand Parent { get; set; }
        public List<ICommand> Children { get; set; }
        UniTask ICommand.Begin(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}