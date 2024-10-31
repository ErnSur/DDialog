namespace QuickEye.PeeDialog
{
    using System.Threading;
    using UnityEngine;

    public class DialogEngine
    {
        public CommandRunner CommandRunner { get; } = new CommandRunner();

        protected readonly IPrinter Printer;

        public DialogEngine(IPrinter printer)
        {
            Printer = printer;
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

        public async Awaitable Print(string script, CancellationToken cancellationToken = default)
        {
            await CommandRunner.Execute(script, cancellationToken);
        }
    }
}