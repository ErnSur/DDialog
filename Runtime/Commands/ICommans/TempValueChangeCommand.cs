namespace Doublsb.Dialog
{
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public abstract class TempValueChangeCommand<T> : ICommand
    {
        protected T NewValue { get; set; }
        protected abstract T Value { get; set; }
        private T _previousValue;
        
        public UniTask Begin(DialogCommandSet dialogCommandSet, CancellationToken fastForwardToken,
            CancellationToken cancellationToken)
        {
            _previousValue = Value;
            Value = NewValue;
            return UniTask.CompletedTask;
        }
        
        public UniTask End(DialogCommandSet dialogCommandSet, CancellationToken fastForwardToken,
            CancellationToken cancellationToken)
        {
            Value = _previousValue;
            return UniTask.CompletedTask;
        }
    }
}