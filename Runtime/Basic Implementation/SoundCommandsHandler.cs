namespace QuickEye.PeeDialog
{
    using UnityEngine;

    [DefaultExecutionOrder(DefaultExecutionOrder)]
    public class SoundCommandsHandler : MonoBehaviour
    {
        public const int DefaultExecutionOrder = ActorCommandsHandler.DefaultExecutionOrder + 1;
        protected ISoundManager SoundManager;
        protected CommandRunner CommandRunner;

        protected virtual void Awake()
        {
            SoundManager = GetComponent<ISoundManager>();
            CommandRunner = GetComponent<ICommandRunnerProvider>().CommandRunner;
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