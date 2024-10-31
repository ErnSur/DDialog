namespace QuickEye.PeeDialog
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using UnityEngine;

    public class AsyncUtils
    {
        public static async Awaitable Delay(float seconds, CancellationToken cancellationToken = default)
        {
            await Awaitable.WaitForSecondsAsync(seconds, cancellationToken);
        }
        
        public static async Awaitable Delay(TimeSpan timeSpan, CancellationToken cancellationToken = default)
        {
            await Delay((float)timeSpan.TotalSeconds, cancellationToken);
        }

        public static async Task WaitUntilCanceled(CancellationToken skipCtsToken)
        {
            await WaitUntil(() => skipCtsToken.IsCancellationRequested);
        }

        public static async Awaitable WaitUntil(Func<bool> func, CancellationToken cancellationToken = default)
        {
            while (!func())
            {
                await Awaitable.NextFrameAsync(cancellationToken);
            }
        }
    }
}