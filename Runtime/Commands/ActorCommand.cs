namespace Doublsb.Dialog
{
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public class ActorCommand : Command, ICommand
    {
        public string ActorId { get; }
        private readonly BasicActorManager _actorManager;

        public ActorCommand(BasicActorManager actorManager, string actorId)
        {
            _actorManager = actorManager;
            ActorId = actorId;
        }
        
        UniTask ICommand.Begin(CancellationToken cancellationToken)
        {
            // TODO: DO hide on end
            _actorManager.Show(ActorId);
            return UniTask.CompletedTask;
        }
    }
}