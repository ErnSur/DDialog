namespace Doublsb.Dialog
{
    using UnityEngine;

    [RequireComponent(typeof(DialogSystem))]
    [RequireComponent(typeof(PrintCommandHandler))]
    internal class BasicSoundEffectManager : MonoBehaviour
    {
        private ComponentPool<AudioSource> _audioSourcesPool;

        private AudioSource _activeAudioSource;
        public float minPlaybackTime = 0.2f;
        
        private ISoundEffectProvider _soundEffectProvider;
        public AudioClip[] defaultChatSoundEffects;
        private DialogSystem _dialogSystem;
        private void Awake()
        {
            _soundEffectProvider = gameObject.GetComponent<ISoundEffectProvider>();
            _dialogSystem = GetComponent<DialogSystem>();

            var printCommandHandler = gameObject.GetComponent<PrintCommandHandler>();
            printCommandHandler.CharacterPrinted += OnCharacterPrinted;
            
            _audioSourcesPool = new ComponentPool<AudioSource>(gameObject, 5);
        }

        private void OnCharacterPrinted(char character)
        {
            if (character != ' ')
                Play_ChatSE();
        }
        
        // Idea: play a different sound on samogłoska i spółgłoska
        // albo inny dźwięk na każdą literę
        public void Play_ChatSE()
        {
            var dialogData = _dialogSystem.CurrentActorLines;
            var clips = defaultChatSoundEffects;
            if (_soundEffectProvider != null &&
                _soundEffectProvider.TryGetChatSoundEffects(dialogData.ActorId, out var actorClips))
                clips = actorClips;
            if (clips == null || clips.Length == 0)
                return;

            var clip = clips[Random.Range(0, clips.Length)];
            if (clip != null)
                PlaySound(clip);
        }

        public void PlaySound(AudioClip clip)
        {
            if (_activeAudioSource != null)
            {
                if (_activeAudioSource.isPlaying && _activeAudioSource.time < minPlaybackTime)
                    return;
                _activeAudioSource.Stop();
                var newAudioSource = _audioSourcesPool.Rent();
                _audioSourcesPool.Return(_activeAudioSource);
                _activeAudioSource = newAudioSource;
            }
            else
            {
                _activeAudioSource = _audioSourcesPool.Rent();
            }
            
            _activeAudioSource.clip = clip;
            _activeAudioSource.Play();
        }
    }
}