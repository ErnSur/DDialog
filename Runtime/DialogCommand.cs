namespace Doublsb.Dialog
{
    public class DialogCommand
    {
        public readonly CommandId CommandId;
        public readonly string Argument;

        public DialogCommand(CommandId commandId, string argument = "")
        {
            CommandId = commandId;
            Argument = argument;
        }
    }
}