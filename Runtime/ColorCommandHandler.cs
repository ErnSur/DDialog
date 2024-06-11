namespace Doublsb.Dialog
{
    using System.Collections;

    internal class ColorCommandHandler : IDialogCommandHandler
    {
        public string Identifier => "color";

        public IEnumerator PerformAction(string context, DialogData dialogData)
        {
            dialogData.Format.Color = context;
            yield break;
        }
    }
}