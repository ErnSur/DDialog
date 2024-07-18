namespace Doublsb.Dialog
{
    public interface ICommandFactory
    {
        bool TryGetCommand(string commandId, string[] args, out ICommand command);
    }
}