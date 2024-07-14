namespace Doublsb.Dialog
{
    using System.Collections.Generic;
    using UnityEngine;

    public interface IAudioPlayer
    {
        void PlaySound(AudioClip clip);
    }

    internal class SoundManager : MonoBehaviour, IAudioPlayer
    {
        private ComponentPool<AudioSource> _audioSourcesPool;

        private AudioSource _activeAudioSource;
        public float minPlaybackTime = 0.2f;
        private void Awake()
        {
            _audioSourcesPool = new ComponentPool<AudioSource>(gameObject, 5);
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

    internal class ComponentPool<T> where T : Component
    {
        private readonly GameObject _gameObject;

        private readonly int _startSize;

        private readonly Stack<T> _available = new Stack<T>();
        private readonly HashSet<T> _rented = new HashSet<T>();

        public ComponentPool(GameObject gameObject, int size)
        {
            _gameObject = gameObject;
            _startSize = size;
            Initialize();
        }

        public int CountAll => CountRented + CountAvailable;
        public int CountRented => _rented.Count;
        public int CountAvailable => _available.Count;

        public void Initialize()
        {
            for (var i = 0; i < _startSize; i++)
                _available.Push(CreateObject());
        }

        public virtual T Rent()
        {
            var obj = _available.Count > 0 ? _available.Pop() : CreateObject();
            _rented.Add(obj);
            if (obj is Behaviour behaviour)
                behaviour.enabled = true;
            return obj;
        }

        public void Return(T obj)
        {
            if (_available.Contains(obj))
            {
                Debug.LogWarning("Trying to return already released object");
                return;
            }

            _rented.Remove(obj);

            if (obj is Behaviour behaviour)
                behaviour.enabled = false;

            _available.Push(obj);
        }

        public void ReturnAll()
        {
            foreach (var obj in _rented)
                Return(obj);
        }

        private T CreateObject()
        {
            var newObject = _gameObject.AddComponent<T>();
            if (newObject is Behaviour behaviour)
                behaviour.enabled = false;
            return newObject;
        }
    }
}