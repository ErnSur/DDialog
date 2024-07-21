namespace QuickEye.PeeDialog
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MenuOption
    {
        public readonly string Text;
        public readonly Action Callback;

        public MenuOption(string text, Action callback)
        {
            Text = text;
            Callback = callback;
        }
    }

    public class Menu
    {
        public Menu(string id)
        {
            Id = id;
        }

        public string Id { get; }
        public List<MenuOption> Options { get; } = new();
        public override string ToString()
        {
            return $"<menu=\"{Id}\" options=\"{Options.Select(o=>o.Text)}\"/>";
        }
    }
}