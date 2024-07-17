namespace Doublsb.Dialog
{
    using System.Collections.Generic;
    using System.Xml.Linq;
    using UnityEngine;

    [RequireComponent(typeof(IPrinter))]
    public class BasicCommandFactory : MonoBehaviour, ICommandFactory
    {
        private IPrinter _printer;

        private void Awake()
        {
            _printer = GetComponent<IPrinter>();
        }

        public List<Command> GetCommands(XElement commandTree)
        {
            var result = new List<Command>();
            AddCommands(commandTree, result);
            return result;
        }

        private void AddCommands(XElement root, List<Command> commands)
        {
            // Tag start callback
            if (TryGetCommand(root.Name.LocalName, root.FirstAttribute?.Value, out var command))
            {
                commands.Add(command);
            }

            foreach (var node in root.Nodes())
            {
                switch (node)
                {
                    case XText xText:
                        Debug.Log($"TryGetCommand: print, {xText.Value}");

                        commands.Add(new PrintCommand(_printer, xText.Value));
                        break;
                    case XElement xElement:
                        AddCommands(xElement, commands);
                        break;
                }
            }

            // Tag ends callback
            if (!root.IsEmpty && command != null)
            {
                commands.Add(command);
            }
        }
        private bool TryGetCommand(string commandId, string arg1, out Command command)
        {
            Debug.Log($"TryGetCommand: {commandId}, {arg1}");
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
}