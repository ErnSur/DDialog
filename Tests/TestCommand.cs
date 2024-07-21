namespace Tests
{
    using System.Threading;
    using Cysharp.Threading.Tasks;

    // internal class TestCommand : ICommand
    // {
    //     public bool Began, Ended, Disposed;
    //
    //     protected override async UniTask Begin(CancellationToken cancellationToken)
    //     {
    //         await UniTask.Delay(1);
    //         Began = true;
    //     }
    //
    //     protected override async UniTask End(CancellationToken cancellationToken)
    //     {
    //         await UniTask.Delay(1);
    //         Ended = true;
    //     }
    //
    //     public override void Dispose()
    //     {
    //         Disposed = true;
    //     }
    // }
}