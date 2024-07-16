namespace Doublsb.Dialog
{
    using System.Collections;
    using System.Threading;
    using UnityEngine;

    internal class SoundCommandHandler : MonoBehaviour, IDialogCommandHandler
    {
        public string Identifier => "sound";

        [SerializeField]
        private UnityDictionary<string, AudioClip> sounds;

        [SerializeField]
        private AudioSource audioSource;
        
        private void Awake()
        {
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }

        public IEnumerator PerformAction(string soundId, DialogCommandSet dialogCommandSet, CancellationToken fastForwardToken)
        {
            if (!sounds.TryGetValue(soundId, out var clip))
            {
                Debug.LogError($"Sound not found: {soundId}");
                yield break;
            }

            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}