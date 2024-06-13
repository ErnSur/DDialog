namespace Doublsb.Dialog
{
    using System.Collections;

    public interface IDialogCommandHandler
    {
        string Identifier { get; }
        IEnumerator PerformAction(string context, DialogData dialogData);
    }
}