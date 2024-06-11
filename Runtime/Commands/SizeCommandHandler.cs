namespace Doublsb.Dialog
{
    using System.Collections;
    using UnityEngine;

    public class SizeCommandHandler : MonoBehaviour, IDialogCommandHandler
    {
        public string Identifier => "size";
        public IEnumerator PerformAction(string context, DialogData dialogData)
        {
            dialogData.Format.Resize(context);
            yield break;
        }
    }
}