namespace Doublsb.Dialog
{
    using System;
    using System.Collections.Generic;
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
            var printText = string.Empty;

            for (int i = 0; i < textWithCommands.Length; i++)
            {
                if (textWithCommands[i] != '/')
                    printText += textWithCommands[i];
                else // If find '/'
                {
                    // Convert last printText to command
                    if (printText != string.Empty)
                    {
                        Commands.Add(new DialogCommand(CommandId.print, printText));
                        printText = string.Empty;
                    }

                    // Extract the command content, (i.e., "/commandName:argument/")
                    var nextSlashIndex = textWithCommands.IndexOf('/', i + 1);
                    var commandContent = textWithCommands.Substring(i + 1, nextSlashIndex - i - 1);

                    // Add new command
                    if(TryParseCommand(commandContent,out var command))
                        Commands.Add(command);

                    // Move loop index
                    i = nextSlashIndex;
                }
            }

            if (printText != string.Empty)
                Commands.Add(new DialogCommand(CommandId.print, printText));
        }

        private static bool TryParseCommand(string text, out DialogCommand command)
        {
            var segments = text.Split(':');

            if (Enum.TryParse(segments[0], out CommandId commandId))
            {
                command = segments.Length >= 2 ? new DialogCommand(commandId, segments[1]) : new DialogCommand(commandId);
                return true;
            }

            Debug.LogError($"Cannot parse command content: {text}");
            command = null;
            return false;
        }
    }
}