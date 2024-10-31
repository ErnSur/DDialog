namespace QuickEye.PeeDialog
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDialogMenuView
    {
        public event Action<int> OptionSelected;
        public Task<int> Open(IReadOnlyList<string> options);
        public Task Close();
    }
}