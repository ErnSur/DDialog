namespace Doublsb.Dialog
{
    using System;
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
 
            using var skipCts = BasicPrinter.CreateSkipCts(cancellationToken);
            await UniTask.Delay(TimeSpan.FromSeconds(_waitTime), cancellationToken: skipCts.Token);
        }
    }
}