namespace Doublsb.Dialog
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using UnityEngine;

    internal static class CommandParser
    {
        public static List<Command> Parse(string text, ICommandFactory commandFactory)
        {
            try
            {
                text = ReplaceShorthandXmlTags(text);
                var xDoc = XDocument.Parse($"<root>{text}</root>");
                var commandDef = XmlToCommandDefinition(xDoc.Root);
                var commands = commandFactory.GetCommands(commandDef);
                return commands;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error parsing dialog: {text}\n{e}");
            }

            return new List<Command>();
        }

        private static CommandDefinition XmlToCommandDefinition(XElement xmlRoot)
        {
            // Tag start callback
            var root = new CommandDefinition
            {
                name = xmlRoot.Name.LocalName,
                args = xmlRoot.Attributes().Select(a => a.Value).ToArray()
            };

            foreach (var node in xmlRoot.Nodes())
            {
                switch (node)
                {
                    case XText xText:
                        root.children.Add(new CommandDefinition("print", xText.Value));
                        break;
                    case XElement xElement:
                        root.children.Add(XmlToCommandDefinition(xElement));
                        break;
                }
            }

            return root;
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