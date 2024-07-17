namespace Doublsb.Dialog
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    internal class PeeDialogScriptParser
    {
        public static List<Func<UniTask>> Parse(string text, ICommandFactory commandFactory)
        {
            try
            {
                textWithCommands = ReplaceShorthandXmlTags(textWithCommands);
                var xDoc = XDocument.Parse($"<root>{textWithCommands}</root>");
                AddCommands(xDoc.Root);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error parsing dialog: {textWithCommands}\n{e}");
            }
        }
        
        private void AddCommands(XElement root, List<Func<UniTask>> commands, ICommandFactory commandFactory)
        {
            // Tag start callback
            if (commandFactory.TryGetCommand(root, out var command))
            {
                commands.Add(command);
            }

            foreach (var node in root.Nodes())
            {
                switch (node)
                {
                    case XText xText:
                        Commands.Add(new CommandDescriptor("print", xText.Value));
                        break;
                    case XElement xElement:
                        AddCommands(xElement);
                        break;
                }
            }

            // Tag ends callback
            if (!root.IsEmpty && command != null)
            {
                Commands.Add(new CommandDescriptor(command.Id, "end"));
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
    }
}