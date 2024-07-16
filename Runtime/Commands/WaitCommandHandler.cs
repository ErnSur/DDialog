namespace Doublsb.Dialog
{
    using System.Collections;
    using UnityEngine;

    internal class WaitCommandHandler : MonoBehaviour, IDialogCommandHandler
    {
        public string Identifier => "wait";

        public IEnumerator PerformAction(string context, DialogData dialogData)
        {
            if (float.TryParse(context, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var waitTime))
                yield return new WaitForSeconds(waitTime);
            else
                Debug.LogError($"Cannot parse float number: {context}");
        }
    }
}