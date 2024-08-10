namespace QuickEye.PeeDialog
{
    using System.Linq;
    using JetBrains.Annotations;
    using UnityEngine;

    [PublicAPI]
    public class ActorCommandsHandler
    {
        protected IActorManager ActorManager;
        protected CommandRunner CommandRunner;

        public ActorCommandsHandler(CommandRunner commandRunner, IActorManager actorManager)
        {
            CommandRunner = commandRunner;
            ActorManager = actorManager;
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