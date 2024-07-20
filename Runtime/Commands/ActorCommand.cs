namespace Doublsb.Dialog
{
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class ActorCommand : Command, ICommand
    {
        public string ActorId { get; }
        private readonly BasicActorManager _actorManager;
        private IPrinter _printer;

        public ActorCommand(IPrinter printer, BasicActorManager actorManager, string actorId)
        {
            _printer = printer;
            _actorManager = actorManager;
            ActorId = actorId;
        }

        UniTask ICommand.Begin(CancellationToken cancellationToken)
        {
            // TODO: DO hide on end
            _actorManager.StartActorLine(ActorId);
            _printer.Reset();
            _printer.SetActive(true);
            _actorManager.Show(ActorId);
            return UniTask.CompletedTask;
        }

        async UniTask ICommand.End(CancellationToken cancellationToken)
        {
            // await to next frame, so we are not consuming someone else's mouse click
            //await UniTask.NextFrame();
            using var skipCts = BasicPrinter.CreateSkipCts(cancellationToken);
            await UniTask.WaitUntilCanceled(skipCts.Token);
            // await to next frame so other commands won't consume this mouse click
            //await UniTask.NextFrame();
            Debug.Log($"Actor Line ended. Canceled: {cancellationToken.IsCancellationRequested}");
            _actorManager.EndActorLine();
            _printer.SetActive(false);
        }
    }
}