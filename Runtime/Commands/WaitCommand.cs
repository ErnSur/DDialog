namespace Doublsb.Dialog
{
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class WaitCommand : Command, ICommand
    {
        private readonly float _waitTime;

        public WaitCommand(string waitTime)
        {
            if (!float.TryParse(waitTime,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out _waitTime))
                Debug.LogError($"Cannot parse float number: {waitTime}");
        }

        async UniTask ICommand.Begin(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            await UniTask.WaitForSeconds(_waitTime, false, PlayerLoopTiming.Update, cancellationToken);
        }
    }
}