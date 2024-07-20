namespace Doublsb.Dialog
{
    using UnityEngine;

    public class SoundCommandsHandler : MonoBehaviour
    {
        protected AudioSource AudioSource;
        protected UnityDictionary<string, AudioClip> Sounds=new();
        protected CommandRunner CommandRunner;

        protected virtual void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
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

                var soundId = args[0];
                if (!Sounds.TryGetValue(soundId, out var clip))
                {
                    Debug.LogError($"Sound with id {soundId} not found");
                    return;
                }

                AudioSource.PlayOneShot(clip);
            });
        }
    }
}