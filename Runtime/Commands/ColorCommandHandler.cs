namespace Doublsb.Dialog
{
    using System.Collections;
    using System.Threading;
    using UnityEngine;

    internal class ColorCommandHandler : MonoBehaviour, IDialogCommandHandler
    {
        public string Identifier => "color";

        public IEnumerator PerformAction(string context, DialogCommandSet dialogCommandSet, CancellationToken fastForwardToken)
        {
            dialogCommandSet.Format.Color = context;
            yield break;
        }
    }
}