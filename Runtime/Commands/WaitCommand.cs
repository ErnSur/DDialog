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
            //try
            //{
                Debug.Log($"Wait Thread: {Thread.CurrentThread.ManagedThreadId} / {cancellationToken.IsCancellationRequested}");
                await UniTask.WaitForSeconds(1, false, PlayerLoopTiming.Update, cancellationToken);
                Debug.Log($"Wait finished: {Thread.CurrentThread.ManagedThreadId}");
            // }
            // catch (Exception e)
            // {
                // Debug.LogError(e);
            // }
        }
    }
}