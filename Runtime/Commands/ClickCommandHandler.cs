namespace Doublsb.Dialog
{
    using System.Collections;
    using System.Threading;
    using System.Threading.Tasks;
    using UnityEngine;

    internal class ClickCommandHandler : MonoBehaviour, IDialogCommandHandler
    {
        public string Identifier => "click";

        public IEnumerator PerformAction(string context, ActorLines actorLines, CancellationToken fastForwardToken)
        {
            while (!Input.GetMouseButtonDown(0) && !fastForwardToken.IsCancellationRequested)
                yield return null;
        }
    }
}