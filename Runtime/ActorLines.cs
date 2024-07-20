namespace Doublsb.Dialog
{
    using System;
    using UnityEngine.Events;
    using System.Collections.Generic;

    public class ActorLines
    {
        public readonly string Script;
        public readonly string ActorId;
        public readonly bool CanBeSkipped;
        public UnityAction Callback;

        public ActorLines(string script, string actorId = "", UnityAction callback = null, bool canBeSkipped = true)
        {
            Script = script;
            ActorId = actorId;
            CanBeSkipped = canBeSkipped;
            Callback = callback;
        }

        [Obsolete("Create Menu selection as a cusom command")]
        public List<MenuOption> SelectList { get; set; } = new List<MenuOption>();
    }
}