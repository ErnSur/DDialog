namespace QuickEye.PeeDialog
{
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public class DialogEngine
    {
        public CommandRunner CommandRunner { get; } = new CommandRunner();

        public DialogEngine(IPrinter printer)
        {
            new PrintCommandsHandler(CommandRunner, printer);
        }
        
        public void AddSoundSupport(ISoundManager soundManager)
        {
            new SoundCommandsHandler(CommandRunner, soundManager);
        }

        public void AddActorSupport(IActorManager actorManager)
        {
            new ActorCommandsHandler(CommandRunner, actorManager);
        }

        public async UniTask Print(string script,CancellationToken cancellationToken=default)
        {
            await CommandRunner.Execute(script,cancellationToken);
        }
    }
}