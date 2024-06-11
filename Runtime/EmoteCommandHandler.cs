namespace Doublsb.Dialog
{
    using System.Collections;using UnityEngine;

    internal class EmoteCommandHandler : MonoBehaviour, IDialogCommandHandler
    {
        public string Identifier => "emote";
        
        private IDialogActorManager _actorManager;

        private void Awake()
        {
            _actorManager = GetComponent<IDialogActorManager>();
        }

        public IEnumerator PerformAction(string emoteId, DialogData dialogData)
        {
            _actorManager.Emote(emoteId);
            yield break;
        }
    }
}