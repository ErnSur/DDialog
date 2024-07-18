namespace Doublsb.Dialog
{
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class SoundCommand : ICommand
    {
        private readonly AudioSource _audioSource;
        private readonly AudioClip _sound;

        public SoundCommand(AudioClip sound, AudioSource audioSource)
        {
            _sound = sound;
            _audioSource = audioSource;
        }

        public UniTask Begin(CancellationToken cancellationToken)
        {
            _audioSource.PlayOneShot(_sound);
            return UniTask.CompletedTask;
        }
    }
}