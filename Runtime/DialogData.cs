namespace Doublsb.Dialog
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// Convert string to Data. Contains List of DialogCommand and DialogFormat.
    /// </summary>
    public class DialogData
    {
        public readonly string ActorId;
        public readonly List<DialogCommand> Commands = new List<DialogCommand>();
        public readonly DialogSelect SelectList = new DialogSelect();
        public readonly DialogFormat Format = new DialogFormat();
        public readonly AudioClip[] ChatSoundEffects;

        public string PrintText = string.Empty;

        public readonly bool CanBeSkipped;
        public UnityAction Callback;

        public DialogData(string originalString, string actorId = "", UnityAction callback = null,
            bool canBeSkipped = true, AudioClip[] chatSoundEffects = null)
        {
            Initialize(originalString);

            ChatSoundEffects = chatSoundEffects;
            CanBeSkipped = canBeSkipped;
            Callback = callback;
            ActorId = actorId;
        }

        private void Initialize(string textWithCommands)
        {
            try
            {
                var xDoc = XDocument.Parse($"<root>{textWithCommands}</root>");
            ParseCommands(xDoc.Root);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error parsing dialog: {textWithCommands}\n{e}");
            }
        }

        private void ParseCommands(XElement root)
        {
            // Tag start callback
            Console.Write($"/{root.Name}/");
            if (root.Name != "root" && TryParseCommand(root, out var command))
            {
                Commands.Add(command);
                return;
            }
            foreach (var node in root.Nodes())
            {
                switch (node)
                {
                    case XText xText:
                        // Print command
                        //Console.Write(xText.Value);
                        Commands.Add(new DialogCommand(CommandId.print, xText.Value));
                        break;
                    case XElement xElement:
                        ParseCommands(xElement);
                        break;
                }
            }
            // Tag ends callback
        }

        private static bool TryParseCommand(XElement commandElement, out DialogCommand command)
        {
            var commandName = commandElement.Name.LocalName;

            if (Enum.TryParse(commandName, out CommandId commandId))
            {
                var firstAttribute = commandElement.FirstAttribute;
                command = firstAttribute != null ? 
                    new DialogCommand(commandId, firstAttribute.Value) :
                    new DialogCommand(commandId);
                return true;
            }

            Debug.LogError($"Cannot parse command content: {commandName}");
            command = null;
            return false;
        }
    }
}