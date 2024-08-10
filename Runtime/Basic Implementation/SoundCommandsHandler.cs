namespace QuickEye.PeeDialog
{
    using JetBrains.Annotations;
    using UnityEngine;

    [PublicAPI]
    public class SoundCommandsHandler
    {
        protected ISoundManager SoundManager;
        protected CommandRunner CommandRunner;

        public SoundCommandsHandler(CommandRunner commandRunner, ISoundManager soundManager)
        {
            SoundManager = soundManager;
            CommandRunner = commandRunner;
            RegisterCommands();
        }

        protected virtual void RegisterCommands()
        {
            RegisterSound();
        }

        protected virtual void RegisterSound()
        {
            CommandRunner.RegisterCommandCallback("sound", async (args, token) =>
            {
                if (args.Length == 0)
                {
                    Debug.LogError("Sound command requires an argument");
                    return;
                }
                SoundManager.PlaySound(args[0]);
            });
        }
    }
}