namespace Doublsb.Dialog
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class CommandDefinition
    {
        public string name;
        public string[] args = Array.Empty<string>();
        public List<CommandDefinition> children = new List<CommandDefinition>();
        public CommandDefinition()
        {
        }

        public CommandDefinition(string name, params string[] strings)
        {
            this.name = name;
            args = strings;
        }

        public bool IsEmpty => children?.Count == 0;
    }
}