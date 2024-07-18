namespace Doublsb.Dialog
{
    using System.Collections.Generic;
    using UnityEngine.Events;

    public class ActorLines
    {
        public readonly string Script;
        public readonly string ActorId;
        public readonly bool CanBeSkipped;
        public readonly List<MenuOption> SelectList = new List<MenuOption>();
        public UnityAction Callback;

        public ActorLines(string script, string actorId = "", UnityAction callback = null, bool canBeSkipped = true)
        {
            Script = script;
            ActorId = actorId;
            CanBeSkipped = canBeSkipped;
            Callback = callback;
        }
    }
}