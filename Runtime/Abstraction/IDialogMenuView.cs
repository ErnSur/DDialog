namespace Doublsb.Dialog
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;

    public interface IDialogMenuView
    {
        public event Action<int> OptionSelected;
        public UniTask<int> Open(IReadOnlyList<string> options);
        public UniTask Close();
    }
}