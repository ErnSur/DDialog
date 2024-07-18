namespace Doublsb.Dialog
{
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class SoundCommand : Command, ICommand
    {
        private readonly AudioSource _audioSource;
        private readonly AudioClip _sound;

        public SoundCommand(AudioClip sound, AudioSource audioSource)
        {
            _sound = sound;
            _audioSource = audioSource;
        }

        UniTask ICommand.Begin(CancellationToken cancellationToken)
        {
            _audioSource.PlayOneShot(_sound);
            return UniTask.CompletedTask;
        }
    }
}