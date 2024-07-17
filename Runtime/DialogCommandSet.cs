namespace Doublsb.Dialog
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using UnityEngine;
    using UnityEngine.Events;
    
    public class DialogCommandSet
    {
        public readonly string ActorId;
        public readonly List<DialogCommand> Commands = new List<DialogCommand>();
        public readonly List<MenuOption> SelectList = new List<MenuOption>();
        public readonly AudioClip[] ChatSoundEffects;

        public string PrintText = string.Empty;

        public readonly bool CanBeSkipped;
        public UnityAction Callback;

        public DialogCommandSet(string originalString, string actorId = "", UnityAction callback = null,
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
                textWithCommands = ReplaceShorthandXmlTags(textWithCommands);
                var xDoc = XDocument.Parse($"<root>{textWithCommands}</root>");
                ParseCommands(xDoc.Root);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error parsing dialog: {textWithCommands}\n{e}");
            }
        }

        /// <summary>
        /// Replace shorthand XML tags with full XML tags.
        /// <example>
        /// Turn: &lt;color=red&gt;text&lt;/color&gt;
        /// Into &lt;color color=&quot;red&quot;&gt;text&lt;/color&gt;
        /// </example>
        /// </summary>
        private static string ReplaceShorthandXmlTags(string textWithCommands)
        {
            // Elements with shorthand syntax
            // Example: <color=red>text</color>
            // Group 1: tag name
            // Group 2: =
            // Group 3: value (without quotes) (only if quotes are missing)
            var xmlShorthandElementsRegex =
                @"<(?:(\w+):)?(?<tagName>\w+(?:\.\w+)?)(?:=(?<value>[^""'\s/>]+)|=[""'](?:[^""']*?)[""'])";
            var matches = Regex.Matches(textWithCommands, xmlShorthandElementsRegex);
            // Reverse the matches so that we can insert the spaces and quotes without messing up the indexes
            foreach (Match match in matches.Reverse())
            {
                var tagNameGroup = match.Groups["tagName"];
                var valueGroup = match.Groups["value"];

                // put a space with a tag name before the "=" and add quotes around value if value mach is present
                // do it from the end to the start to not mess up the indexes
                if (valueGroup.Success)
                {
                    textWithCommands = textWithCommands.Insert(valueGroup.Index + valueGroup.Length, "\"");
                    textWithCommands = textWithCommands.Insert(valueGroup.Index, "\"");
                }

                var tagNameEndIndex = tagNameGroup.Index + tagNameGroup.Length;
                textWithCommands = textWithCommands.Insert(tagNameEndIndex, $" {tagNameGroup.Value}");
            }

            return textWithCommands;
        }

        // TODO: the position of the note tag content can be determined by saving the start and end position of the Text component when printing the text.
        private void ParseCommands(XElement root)
        {
            // Tag start callback
            if (TryParseCommand(root, out var command))
            {
                Commands.Add(command);
            }
            foreach (var node in root.Nodes())
            {
                switch (node)
                {
                    case XText xText:
                        Commands.Add(new DialogCommand(CommandId.print, xText.Value));
                        break;
                    case XElement xElement:
                        ParseCommands(xElement);
                        break;
                }
            }
            
            // Tag ends callback
            if (!root.IsEmpty && command != null)
            {
                Commands.Add(new DialogCommand(command.CommandId, "end"));
            }
        }

        private static bool TryParseCommand(XElement commandElement, out DialogCommand command)
        {
            if (commandElement.Name.LocalName == "root")
            {
                command = null;
                return false;
            }
            var commandName = commandElement.Name.LocalName;

            if (Enum.TryParse(commandName, out CommandId commandId))
            {
                command = new DialogCommand(commandId, commandElement.FirstAttribute?.Value);
                return true;
            }

            Debug.LogError($"Cannot parse command content: {commandName}");
            command = null;
            return false;
        }
    }
}