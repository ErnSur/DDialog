namespace Doublsb.Dialog
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class BasicCommandFactory : ICommandFactory
    {
        private readonly IPrinter _printer;

        public BasicCommandFactory(IPrinter printer)
        {
            _printer = printer;
        }

        public List<Command> GetCommands(CommandDefinition commandTree)
        {
            var result = new List<Command>();
            AddCommands(commandTree, result);
            return result;
        }

        private void AddCommands(CommandDefinition root, List<Command> commands)
        {
            // Tag start callback
            if (TryGetCommand(root.name, root.args.FirstOrDefault(), out var command))
            {
                commands.Add(command);
            }

            foreach (var child in root.children)
            {
                AddCommands(child, commands);
            }

            // Tag ends callback
            if (!root.IsEmpty && command != null)
            {
                commands.Add(command);
            }
        }

        private bool TryGetCommand(string commandId, string arg1, out Command command)
        {
//            Debug.Log($"TryGetCommand: {commandId}, {arg1}");
            switch (commandId)
            {
                case "print":
                    command = new PrintCommand(_printer, arg1);
                    return true;
                case "size":
                    command = new SizeCommand(_printer, arg1);
                    return true;
                case "color":
                    command = new ColorCommand(_printer, arg1);
                    return true;
                // case "wait":
                //     command = new WaitCommand(descriptor);
                //     return true;
                case "speed":
                    command = new SpeedCommand(_printer, arg1);
                    return true;
                default:
                    command = null;
                    return false;
            }
        }
    }

    [RequireComponent(typeof(IPrinter))]
    public class BasicCommandFactoryBehaviour : MonoBehaviour, ICommandFactory
    {
        private BasicCommandFactory _factory;

        private void Awake()
        {
            var printer = GetComponent<IPrinter>();
            _factory = new BasicCommandFactory(printer);
        }

        public List<Command> GetCommands(CommandDefinition commandTree)
        {
            return _factory.GetCommands(commandTree);
        }
    }
}