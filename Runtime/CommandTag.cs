namespace Doublsb.Dialog
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class CommandTag
    {
        public string name;
        public string[] args = Array.Empty<string>();
        public List<CommandTag> children = new List<CommandTag>();
        public CommandTag()
        {
        }

        public CommandTag(string name, params string[] strings)
        {
            this.name = name;
            args = strings;
        }

        public bool IsEmpty => children?.Count == 0;
    }
}