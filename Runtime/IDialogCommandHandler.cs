namespace Doublsb.Dialog
{
    using System.Collections;

    interface IDialogCommandHandler
    {
        string Identifier { get; }
        IEnumerator PerformAction(string context, DialogData dialogData);
    }
    
    public class SizeCommandHandler : IDialogCommandHandler
    {
        public string Identifier => "size";
        public IEnumerator PerformAction(string context, DialogData dialogData)
        {
            dialogData.Format.Resize(context);
            yield break;
        }
    }
}