namespace Doublsb.Dialog
{
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public interface ICommand
    {
        /// <summary>
        /// Called on opening tag
        /// </summary>
        UniTask Begin(DialogCommandSet dialogCommandSet, CancellationToken fastForwardToken,
            CancellationToken cancellationToken);

        /// <summary>
        /// Called on closing tag if node had any children
        /// </summary>
        UniTask End(DialogCommandSet dialogCommandSet, CancellationToken fastForwardToken,
            CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// Called when the dialog is finished.
        /// </summary>
        UniTask Cleanup(DialogCommandSet dialogCommandSet,
            CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}