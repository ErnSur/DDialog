namespace QuickEye.PeeDialog
{
    using UnityEngine;

    [RequireComponent(typeof(BasicPrinter))]
    [RequireComponent(typeof(IActorManager))]
    internal class BasicSoundEffectManager : MonoBehaviour, ISoundManager
    {
        public float minPlaybackTime = 0.2f;
        public AudioClip[] defaultChatSoundEffects;

        [SerializeField]
        protected UnityDictionary<string, AudioClip> sounds = new();

        private ComponentPool<AudioSource> _audioSourcesPool;
        private AudioSource _activeAudioSource;
        private AudioSource _oneShotAudioSource;
        private ISoundEffectProvider _soundEffectProvider;
        private BasicPrinter _printer;
        private IActorManager _actorManager;

        private void Awake()
        {
            _soundEffectProvider = gameObject.GetComponent<ISoundEffectProvider>();
            _actorManager = GetComponent<IActorManager>();

            _audioSourcesPool = new ComponentPool<AudioSource>(gameObject, 5);
            _oneShotAudioSource = _audioSourcesPool.Rent();
            _printer = gameObject.GetComponent<BasicPrinter>();
            _printer.TextPrinted += OnCharacterPrinted;
        }

        private void OnCharacterPrinted()
        {
            if (_printer.Text[^1] != ' ')
                PlayChatSfx();
        }

        // Idea: play a different sound on samogłoska i spółgłoska
        // albo inny dźwięk na każdą literę
        private void PlayChatSfx()
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
                PlaySoundWithCooldown(clip);
        }

        private void PlaySoundWithCooldown(AudioClip clip)
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
            Debug.Log($"Playing sound: {clip.name}", clip);
            _activeAudioSource.Play();
        }

        public void PlaySound(string soundId)
        {
            if (!sounds.TryGetValue(soundId, out var clip))
            {
                Debug.LogError($"Sound with id {soundId} not found");
                return;
            }

            _oneShotAudioSource.PlayOneShot(clip);
        }
    }
}