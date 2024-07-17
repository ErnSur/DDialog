namespace Doublsb.Dialog
{
    using System.Collections;
    using System.Threading;
    using UnityEngine;

    [RequireComponent(typeof(PrintCommandHandler))]
    internal class ColorCommandHandler : MonoBehaviour, IDialogCommandHandler
    {
        public string Identifier => "color";
        
        private PrintCommandHandler _printCommandHandler;

        private void Awake()
        {
            _printCommandHandler = GetComponent<PrintCommandHandler>();
        }

        public IEnumerator PerformAction(string context, DialogCommandSet dialogCommandSet, CancellationToken fastForwardToken)
        {
            if (ColorUtility.TryParseHtmlString(context, out var color))
            {
                _printCommandHandler.Color = color;
            }
            else
            {
                Debug.LogError($"Invalid color code: {context}");
            }
            yield break;
        }
    }
}