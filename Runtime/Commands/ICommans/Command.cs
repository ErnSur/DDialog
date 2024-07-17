namespace Doublsb.Dialog
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    
    public delegate UniTask CommandAction(CancellationToken cancellationToken); 

    public abstract class ICommand : IDisposable
    {
        
        /// <summary>
        /// Called on opening tag
        /// </summary>
        UniTask Begin(CancellationToken fastForwardToken);

        /// <summary>
        /// Called on closing tag if node had any children
        /// </summary>
        UniTask End(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// Called when the dialog is finished.
        /// </summary>
        void IDisposable.Dispose()
        {
        }
    }
    
    internal class ActionCommand : ICommand
    {
        private readonly System.Action _action;

        public ActionCommand(System.Action action)
        {
            _action = action;
        }

        public UniTask Begin(DialogCommandSet dialogCommandSet, CancellationToken fastForwardToken,
            CancellationToken cancellationToken)
        {
            _action();
            return UniTask.CompletedTask;
        }
    }   
}