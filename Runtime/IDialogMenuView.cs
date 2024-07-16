namespace Doublsb.Dialog
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface IDialogMenuView
    {
        public event Action<int> OptionSelected;
        public IEnumerator Open(IReadOnlyList<string> options);
        public IEnumerator Close();
    }
}