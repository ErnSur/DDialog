namespace Doublsb.Dialog
{
    using System.Collections;
    using System.Threading;
    using UnityEngine;

    public class SizeCommandHandler : MonoBehaviour, IDialogCommandHandler
    {
        public string Identifier => "size";
        public IEnumerator PerformAction(string context, DialogCommandSet dialogCommandSet, CancellationToken fastForwardToken)
        {
            dialogCommandSet.Format.Resize(context);
            yield break;
        }
    }
}