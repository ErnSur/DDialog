namespace Doublsb.Dialog
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    
    public delegate UniTask CommandAction(CancellationToken cancellationToken); 

    public abstract class Command : IDisposable
    {
        private bool _runEndNext;
        public async UniTask Run(CancellationToken cancellationToken)
        {
            if(!_runEndNext)
            {
                await Begin(cancellationToken);
                _runEndNext = true;
                return;
            }
            await End(cancellationToken);
        }
        /// <summary>
        /// Called on opening tag
        /// </summary>
        protected abstract UniTask Begin(CancellationToken cancellationToken);

        /// <summary>
        /// Called on closing tag if node had any children
        /// </summary>
        protected virtual UniTask End(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// Called when the dialog is finished.
        /// </summary>
        public  void  Dispose()
        {
        }
    }
}