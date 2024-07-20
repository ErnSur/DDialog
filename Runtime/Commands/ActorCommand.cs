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
            await UniTask.WaitUntil(() => Input.GetMouseButtonUp(0), cancellationToken: cancellationToken);
            _actorManager.EndActorLine();
            _printer.SetActive(false);
        }
    }
}