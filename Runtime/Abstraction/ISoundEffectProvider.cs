namespace QuickEye.PeeDialog
{
    using UnityEngine;

    public interface ISoundEffectProvider
    {
        bool TryGetChatSoundEffects(string actorId, out AudioClip[] clips);
    }
    public interface ISoundManager
    {
        void PlaySound(string soundId);
    }
}