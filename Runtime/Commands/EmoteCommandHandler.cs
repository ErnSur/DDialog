namespace Doublsb.Dialog
{
    using System.Collections;using UnityEngine;

    internal class EmoteCommandHandler : MonoBehaviour, IDialogCommandHandler
    {
        public string Identifier => "e";
        
        private DefaultActorManager _actorManager;

        private void Awake()
        {
            _actorManager = GetComponent<DefaultActorManager>();
        }

        // attributes could be used for handling commands?
        // [CommandHandler("emote")]
        // But then how do you handle dynamic command identifiers? do such command should exist even?
        // for something like emoji recognition they should though
        public IEnumerator PerformAction(string emoteId, DialogData dialogData)
        {
            _actorManager.Emote(dialogData.ActorId, emoteId);
            yield break;
        }
    }
}