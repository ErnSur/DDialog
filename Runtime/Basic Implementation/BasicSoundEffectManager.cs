namespace Doublsb.Dialog
{
    using UnityEngine;

    [RequireComponent(typeof(BasicPrinter))]
    [RequireComponent(typeof(IActorManager))]
    internal class BasicSoundEffectManager : MonoBehaviour
    {
        private ComponentPool<AudioSource> _audioSourcesPool;

        private AudioSource _activeAudioSource;
        public float minPlaybackTime = 0.2f;
        
        private ISoundEffectProvider _soundEffectProvider;
        public AudioClip[] defaultChatSoundEffects;
        private BasicPrinter _printer;
        private IActorManager _actorManager;
        private void Awake()
        {
            _soundEffectProvider = gameObject.GetComponent<ISoundEffectProvider>();
            _actorManager = GetComponent<IActorManager>();
            
            _audioSourcesPool = new ComponentPool<AudioSource>(gameObject, 5);
            _printer = gameObject.GetComponent<BasicPrinter>();
            _printer.TextPrinted += OnCharacterPrinted;
        }

        private void OnCharacterPrinted()
        {
            if (_printer.Text[^1] != ' ')
                Play_ChatSE();
        }
        
        // Idea: play a different sound on samogłoska i spółgłoska
        // albo inny dźwięk na każdą literę
        public void Play_ChatSE()
        {
            var currentActorId = _actorManager.ActiveActorId;
            if (string.IsNullOrEmpty(currentActorId))
            {
                Debug.LogError("Current actor id is null or empty");
                return;
            }
            var clips = defaultChatSoundEffects;
            if (_soundEffectProvider != null &&
                _soundEffectProvider.TryGetChatSoundEffects(currentActorId, out var actorClips))
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