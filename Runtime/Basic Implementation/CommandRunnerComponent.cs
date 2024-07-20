namespace Doublsb.Dialog
{
    using UnityEngine;

    public class CommandRunnerComponent : MonoBehaviour, ICommandRunnerProvider
    {
        public CommandRunner CommandRunner { get; } = new CommandRunner();
    }
}