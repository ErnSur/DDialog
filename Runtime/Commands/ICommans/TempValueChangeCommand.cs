namespace Doublsb.Dialog
{
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public abstract class TempValueChangeCommand<T> : ICommand
    {
        protected T NewValue { get; set; }
        protected abstract T Value { get; set; }
        private T _previousValue;

        public UniTask Begin(CancellationToken cancellationToken)
        {
            _previousValue = Value;
            Value = NewValue;
            //Debug.Log($"[{GetType().Name}] Set:{Value}");
            return UniTask.CompletedTask;
        }

        public UniTask End(CancellationToken cancellationToken)
        {
            Value = _previousValue;
            //Debug.Log($"[{GetType().Name}] Set:{Value}");
            return UniTask.CompletedTask;
        }

        public override string ToString()
        {
            var typeName = GetType().Name;
            // remove "command" from the end
            typeName = typeName.Substring(0, typeName.Length - 7);
            typeName = typeName.ToLowerInvariant();
            return $"<{typeName}={NewValue}>";
        }
    }
}