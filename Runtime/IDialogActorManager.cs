namespace Doublsb.Dialog
{
    using UnityEngine;

    public interface IDialogActorManager
    {
        string SpeakingActor { get; }
        
        bool TryGetChatSoundEffects(out AudioClip[] clips);
        void Show(string actorId);
        void HideAll();
        void Emote(string emotion);
    }
}