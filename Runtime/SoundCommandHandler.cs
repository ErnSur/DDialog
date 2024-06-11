namespace Doublsb.Dialog
{
    using System.Collections;
    using UnityEngine;

    internal class SoundCommandHandler : MonoBehaviour, IDialogCommandHandler
    {
        public string Identifier => "sound";

        //[SerializeField]
        private DefaultActorManager _actorManager;

        [SerializeField]
        private AudioSource audioSource;

        private void Awake()
        {
            _actorManager = GetComponent<DefaultActorManager>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }

        public IEnumerator PerformAction(string soundId, DialogData dialogData)
        {
            if (_actorManager == null || !_actorManager.TryGetActorSound(soundId, out var clip))
                yield break;

            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}