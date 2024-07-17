namespace Doublsb.Dialog
{
    using System.Collections;
    using System.Threading;
    using UnityEngine;


    [RequireComponent(typeof(PrintCommandHandler))]
    public class SizeCommandHandler : MonoBehaviour, IDialogCommandHandler
    {
        public string Identifier => "size";
        private PrintCommandHandler _printCommandHandler;
        private FontSize _previousFontSize;

        private void Awake()
        {
            _printCommandHandler = GetComponent<PrintCommandHandler>();
        }

        public IEnumerator PerformAction(string context, DialogCommandSet dialogCommandSet,
            CancellationToken fastForwardToken)
        {
              //  _printCommandHandler.FontSize = FontSize.ParseString(context);
            yield break;
        }
    }
}