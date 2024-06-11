namespace Doublsb.Dialog
{
    using System.Collections;using UnityEngine;

    internal class ColorCommandHandler : MonoBehaviour, IDialogCommandHandler
    {
        public string Identifier => "color";

        public IEnumerator PerformAction(string context, DialogData dialogData)
        {
            dialogData.Format.Color = context;
            yield break;
        }
    }
}