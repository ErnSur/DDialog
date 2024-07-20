namespace Doublsb.Dialog
{
    using System;
    using UnityEngine.Events;
    using System.Collections.Generic;

    public class ActorLines
    {
        public readonly string Script;
        public readonly string ActorId;
        public UnityAction Callback;

        public ActorLines(string script, string actorId = "", UnityAction callback = null)
        {
            Script = script;
            ActorId = actorId;
            Callback = callback;
        }

        [Obsolete("Create Menu selection as a cusom command")]
        public List<MenuOption> SelectList { get; set; } = new List<MenuOption>();
    }
}