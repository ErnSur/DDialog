namespace Doublsb.Dialog
{
    using System;

    [Serializable]
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
}