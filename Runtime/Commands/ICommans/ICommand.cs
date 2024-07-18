namespace Doublsb.Dialog
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public interface ICommand2
    {
        /// <summary>
        /// Called on opening tag and second time if node had any children
        /// </summary>
        UniTask Act(CancellationToken cancellationToken);
    }
    
    public abstract class CommandWithClosingTag : ICommand2
    {
        private bool _runEndNext;
        public async UniTask Act(CancellationToken cancellationToken)
        {
            if (_runEndNext)
                await End(cancellationToken);
            else
            {
                await Begin(cancellationToken);
                _runEndNext = true;
            }
        }
        
        /// <summary>
        /// Called on opening tag
        /// </summary>
        protected abstract UniTask Begin(CancellationToken cancellationToken);

        /// <summary>
        /// Called on closing tag if node had any children
        /// </summary>
        protected abstract UniTask End(CancellationToken cancellationToken);
    }
    
    public interface ICommand : IDisposable
    {
        /// <summary>
        /// Called on opening tag
        /// </summary>
        UniTask Begin(CancellationToken cancellationToken);

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
}