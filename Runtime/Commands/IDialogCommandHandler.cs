namespace Doublsb.Dialog
{
    using System.Collections;
    using System.Threading;

    public interface IDialogCommandHandler
    {
        string Identifier { get; }
        
        IEnumerator PerformAction(string context, DialogCommandSet dialogCommandSet, CancellationToken fastForwardToken);

        /// <summary>
        /// Called when the dialog is finished.
        /// </summary>
        IEnumerator CleanupAction(string context, DialogCommandSet dialogCommandSet)
        {
            yield break;
        }
    }
}