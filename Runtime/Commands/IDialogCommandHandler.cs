namespace Doublsb.Dialog
{
    using System.Collections;
    using System.Threading;

    public interface IDialogCommandHandler
    {
        string Identifier { get; }
        
        IEnumerator PerformAction(string context, ActorLines actorLines, CancellationToken fastForwardToken);

        /// <summary>
        /// Called when the dialog is finished.
        /// </summary>
        IEnumerator CleanupAction(string context, ActorLines actorLines)
        {
            yield break;
        }
    }
}