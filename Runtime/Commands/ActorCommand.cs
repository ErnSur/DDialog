namespace Doublsb.Dialog
{
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public class ActorCommand : Command, ICommand
    {
        private readonly string _actorId;
        private readonly IActorManager _actorManager;
        private readonly IPrinter _printer;

        public ActorCommand(IPrinter printer, IActorManager actorManager, string actorId)
        {
            _printer = printer;
            _actorManager = actorManager;
            _actorId = actorId;
        }

        UniTask ICommand.Begin(CancellationToken cancellationToken)
        {
            _printer.Reset();
            _printer.SetActive(true);
            _actorManager.ActiveActorId = _actorId;
            return UniTask.CompletedTask;
        }

        async UniTask ICommand.End(CancellationToken cancellationToken)
        {
            using var skipCts = BasicPrinter.CreateSkipCts(cancellationToken);
            await UniTask.WaitUntilCanceled(skipCts.Token);
            _actorManager.ActiveActorId = null;
            _printer.Reset();
            _printer.SetActive(false);
        }
    }
}