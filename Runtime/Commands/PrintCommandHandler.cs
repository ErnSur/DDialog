namespace Doublsb.Dialog
{
    using System;
    using System.Collections;
    using System.Threading;
    using UnityEngine;

    [RequireComponent(typeof(IDialogView))]
    internal class PrintCommandHandler : MonoBehaviour, IDialogCommandHandler
    {
        public event Action<char> CharacterPrinted;
        
        private IDialogView _dialogView;
        
        [SerializeField]
        private float delay = 0.02f;

        public float Delay
        {
            get => delay;
            set => delay = value;
        }
        
        public string Identifier => "print";
        private void Awake()
        {
            _dialogView = GetComponent<IDialogView>();
        }

        public IEnumerator PerformAction(string text, DialogCommandSet dialogCommandSet, CancellationToken fastForwardToken)
        {
            dialogCommandSet.PrintText += dialogCommandSet.Format.OpenTagger;

            for (int i = 0; i < text.Length; i++)
            {
                var character = text[i];
                dialogCommandSet.PrintText += character;
                _dialogView.Text = dialogCommandSet.PrintText + dialogCommandSet.Format.CloseTagger;

                CharacterPrinted?.Invoke(character);
                if (!fastForwardToken.IsCancellationRequested && Delay != 0)
                    yield return new WaitForSeconds(Delay);
            }

            dialogCommandSet.PrintText += dialogCommandSet.Format.CloseTagger;
        }
    }
}