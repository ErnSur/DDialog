namespace Doublsb.Dialog
{
    using UnityEngine;
    using UnityEngine.Serialization;

    public class SoundCommandsHandler : MonoBehaviour
    {
        [SerializeField]
        protected AudioSource audioSource;
        [SerializeField]
        protected UnityDictionary<string, AudioClip> sounds=new();
        protected CommandRunner CommandRunner;

        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
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
                if (!sounds.TryGetValue(soundId, out var clip))
                {
                    Debug.LogError($"Sound with id {soundId} not found");
                    return;
                }

                audioSource.PlayOneShot(clip);
            });
        }
    }
}