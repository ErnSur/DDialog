namespace Doublsb.Dialog
{
    using System.Collections;
    using UnityEngine;

    internal class ClickCommandHandler : MonoBehaviour, IDialogCommandHandler
    {
        public string Identifier => "click";

        public IEnumerator PerformAction(string context, DialogData dialogData)
        {
            yield return WaitForMouseClick();
        }
        
        private static IEnumerator WaitForMouseClick()
        {
            while (!Input.GetMouseButtonDown(0))
                yield return null;
        }
    }
}