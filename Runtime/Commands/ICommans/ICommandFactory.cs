namespace Doublsb.Dialog
{
    using System;

    public interface ICommandFactory
    {
        public bool TryCreateCommand(CommandDescriptor descriptor, out ICommand command)
        {
            switch (descriptor.Id)
            {
                case "size":
                    return new SizeCommand();
                case "print":
                    return new PrintCommand();
                default:
                    throw new ArgumentOutOfRangeException(nameof(descriptor), descriptor, null);
            }
        }
    }
}