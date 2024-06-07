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
        //================================================
        //Public Variable
        //================================================
        public string Character;
        public List<DialogCommand> Commands = new List<DialogCommand>();
        public DialogSelect SelectList = new DialogSelect();
        public DialogFormat Format = new DialogFormat();

        public string PrintText = string.Empty;

        public bool isSkippable = true;
        public UnityAction Callback = null;

        //================================================
        //Public Method
        //================================================
        public DialogData(string originalString, string character = "", UnityAction callback = null, bool isSkipable = true)
        {
            _convert(originalString);

            this.isSkippable = isSkipable;
            this.Callback = callback;
            this.Character = character;
        }

        //================================================
        //Private Method
        //================================================
        private void _convert(string originalString)
        {
            string printText = string.Empty;

            for (int i = 0; i < originalString.Length; i++)
            {
                if (originalString[i] != '/') printText += originalString[i];

                else // If find '/'
                {
                    // Convert last printText to command
                    if (printText != string.Empty)
                    {
                        Commands.Add(new DialogCommand(Command.print, printText));
                        printText = string.Empty;
                    }

                    // Substring /CommandSyntex/
                    var nextSlashIndex = originalString.IndexOf('/', i + 1);
                    string commandSyntex = originalString.Substring(i + 1, nextSlashIndex - i - 1);

                    // Add converted command
                    var com = _convert_Syntex_To_Command(commandSyntex);
                    if (com != null) Commands.Add(com);

                    // Move i
                    i = nextSlashIndex;
                }
            }

            if (printText != string.Empty) Commands.Add(new DialogCommand(Command.print, printText));
        }

        private DialogCommand _convert_Syntex_To_Command(string text)
        {
            var spliter = text.Split(':');

            Command command;
            if (Enum.TryParse(spliter[0], out command))
            {
                if (spliter.Length >= 2) return new DialogCommand(command, spliter[1]);
                else return new DialogCommand(command);
            }
            else
                Debug.LogError("Cannot parse to commands");

            return null;
        }
    }
}