namespace Doublsb.Dialog
{
    using System.Collections.Generic;
    using UnityEngine.Events;

    public class DialogCommandSet
    {
        public readonly string ActorId;
        public readonly string Script;
        public readonly List<MenuOption> SelectList = new List<MenuOption>();

        public readonly bool CanBeSkipped;
        public UnityAction Callback;

        public DialogCommandSet(string originalString, string actorId = "", UnityAction callback = null,
            bool canBeSkipped = true)
        {
            Script = originalString;
            CanBeSkipped = canBeSkipped;
            Callback = callback;
            ActorId = actorId;
        }

        // TODO: the position of the note tag content can be determined by saving the start and end position of the Text component when printing the text
    }
}