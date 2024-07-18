namespace Doublsb.Dialog
{
    using System.Collections.Generic;

    public interface ICommandFactory
    {
        List<Command> GetCommands(CommandTag commandTree);
        private bool TryGetCommand(string commandId, string arg1, out Command command)
    }
}