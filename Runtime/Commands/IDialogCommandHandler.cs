namespace Doublsb.Dialog
{
    using System.Collections;

    interface IDialogCommandHandler
    {
        string Identifier { get; }
        IEnumerator PerformAction(string context, DialogData dialogData);
    }
}