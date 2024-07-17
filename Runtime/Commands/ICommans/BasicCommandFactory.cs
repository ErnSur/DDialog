namespace Doublsb.Dialog
{
    using UnityEngine;

    [RequireComponent(typeof(IPrinter))]
    public class BasicCommandFactory : MonoBehaviour, ICommandFactory
    {
        private IPrinter _printer;
        private PrintSettings _defaultPrintSettings = new PrintSettings();

        private void Awake()
        {
            _printer = GetComponent<IPrinter>();
        }

        public bool TryCreateCommand(CommandDescriptor descriptor, out ICommand command)
        {
            switch (descriptor.Id)
            {
                case "print":
                    command = new PrintCommand(_printer, descriptor.Argument, _defaultPrintSettings);
                    return true;
                case "size":
                    command = new SizeCommand(_printer, descriptor.Argument);
                    return true;
                // case "color":
                //     command = new ColorCommand(descriptor);
                //     return true;
                // case "wait":
                //     command = new WaitCommand(descriptor);
                //     return true;
                // case "speed":
                //     command = new SpeedCommand(descriptor);
                //     return true;
                default:
                    command = null;
                    return false;
            }
        }
    }
}