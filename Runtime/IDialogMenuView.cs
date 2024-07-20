namespace Doublsb.Dialog
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    // TODO: Implement the Menu selection as a custom command
    public interface IDialogMenuView
    {
        public event Action<int> OptionSelected;
        public IEnumerator Open(IReadOnlyList<string> options);
        public IEnumerator Close();
    }
}