namespace Doublsb.Dialog
{
    using UnityEngine;

    public interface ISoundEffectProvider
    {
        bool TryGetChatSoundEffects(string actorId, out AudioClip[] clips);
    }
}