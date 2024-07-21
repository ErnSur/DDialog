namespace QuickEye.PeeDialog
{
    using System.Linq;
    using UnityEngine;

    [DefaultExecutionOrder(DefaultExecutionOrder)]
    [RequireComponent(typeof(ICommandRunnerProvider))]
    public class ActorCommandsHandler : MonoBehaviour
    {
        public const int DefaultExecutionOrder = PrintCommandsHandler.DefaultExecutionOrder + 1;
        protected IActorManager ActorManager;
        protected CommandRunner CommandRunner;

        protected virtual void Awake()
        {
            ActorManager = GetComponent<IActorManager>();
            CommandRunner = GetComponent<ICommandRunnerProvider>().CommandRunner;
            RegisterCommands();
        }

        protected virtual void RegisterCommands()
        {
            RegisterActor();
            RegisterEmote();
        }

        protected virtual void RegisterEmote()
        {
            CommandRunner.RegisterCommandCallback("emote", async (args, token) =>
            {
                ActorManager.Emote(ActorManager.ActiveActorId, args.FirstOrDefault());
            });
        }

        protected virtual void RegisterActor()
        {
            CommandRunner.RegisterCommandCallback("actor", async (args, token) =>
                {
                    ActorManager.ActiveActorId = args.FirstOrDefault();
                },
                async (args, token) =>
                {
                    ActorManager.ActiveActorId = null;
                },new CommandCallbackOptions(0, 10));
        }
    }
}