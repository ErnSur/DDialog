namespace Doublsb.Dialog
{
    using System.Collections.Generic;

    public interface ICommandFactory
    {
        List<Command> GetCommands(CommandDefinition commandTree);
    }
}