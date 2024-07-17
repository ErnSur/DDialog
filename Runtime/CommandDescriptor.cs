namespace Doublsb.Dialog
{
    public class CommandDescriptor
    {
        public readonly string Id;
        public readonly string Argument;

        public CommandDescriptor(string id, string argument = "")
        {
            Id = id;
            Argument = argument;
        }
    }
}