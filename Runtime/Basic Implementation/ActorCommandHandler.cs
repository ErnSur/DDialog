namespace QuickEye.PeeDialog
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using UnityEngine;

    class ActorCommandHandler : ICommandHandler
    {
        readonly IPrinter _printer;

        readonly TimeSpan _skipCooldown = TimeSpan.FromSeconds(0.15f);
        float _lastSkipTime;

        public ActorCommandHandler(IPrinter printer)
        {
            _printer = printer;
        }

        public Task OnBegin(string[] args, CancellationToken cancellationToken)
        {
            _printer.Reset();
            _printer.SetActive(true);
            return Task.CompletedTask;
        }

        public async Task OnEnd(string[] args, CancellationToken cancellationToken)
        {
            await WaitUntilCooldown();
            using var skipCts = BasicPrinter.CreateSkipCts(cancellationToken);
            await AsyncUtils.WaitUntilCanceled(skipCts.Token);
            
            _lastSkipTime = Time.unscaledTime;
            _printer.SetActive(false);
            _printer.Reset();
        }

        /// <summary>
        /// Waits until the cooldown time has passed since the last skip. 
        /// This is to prevent one skip request skipping text printing and the end of the actor command at the same time.
        /// </summary>
        async Task WaitUntilCooldown()
        {
            var timeSinceLastSkip = Time.unscaledTime - _lastSkipTime;
            if (timeSinceLastSkip < _skipCooldown.TotalSeconds)
                await AsyncUtils.Delay(TimeSpan.FromSeconds(_skipCooldown.TotalSeconds - timeSinceLastSkip));
        }
    }
}