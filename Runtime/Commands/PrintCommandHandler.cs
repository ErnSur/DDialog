namespace Doublsb.Dialog
{
    using System;
    using System.Collections;
    using System.Threading;
    using UnityEngine;
    using UnityEngine.UIElements;

    [RequireComponent(typeof(IDialogView))]
    public class PrintCommandHandler : MonoBehaviour, IDialogCommandHandler
    {
        public event Action<char> CharacterPrinted;

        // private IDialogView _dialogView;
        //
        // [SerializeField]
        // private float delay = 0.02f;
        //
        // public float Delay
        // {
        //     get => delay;
        //     set => delay = value;
        // }
        //
        // public Color Color
        // {
        //     get => color;
        //     set
        //     {
        //         color = value;
        //         _format.Color = ColorUtility.ToHtmlStringRGBA(color);
        //     }
        // }
        //
        // public FontSize FontSize
        // {
        //     get => fontSize;
        //     set
        //     {
        //         fontSize = value;
        //         _format.Size = fontSize.ToString();
        //         Debug.Log($"Font size changed to: {fontSize}");
        //     }
        // }
        //
        // private readonly DialogFormat _format = new DialogFormat();
        //
        // [SerializeField]
        // private Color color = Color.white;
        //
        // [SerializeField]
        // private FontSize fontSize = 60;
        //
        // public string Identifier => "print";
        //
        // private string _lastPrintedText;
        // private DialogCommandSet _lastDialogCommandSet;
        //
        // private void Awake()
        // {
        //     _dialogView = GetComponent<IDialogView>();
        // }

        public string Identifier { get; }
        public float Delay { get; set; }
        public Color Color { get; set; }

        public IEnumerator PerformAction(string text, DialogCommandSet dialogCommandSet,
            CancellationToken fastForwardToken)
        {
            yield return null;
            // if(_lastDialogCommandSet != dialogCommandSet)
            // {
            //     _lastPrintedText = string.Empty;
            //     _lastDialogCommandSet = dialogCommandSet;
            // }
            // _lastPrintedText = _format.OpenTagger;
            //
            // for (int i = 0; i < text.Length; i++)
            // {
            //     var character = text[i];
            //     _lastPrintedText += character;
            //     _dialogView.Text = _lastPrintedText + _format.CloseTagger;
            //
            //     CharacterPrinted?.Invoke(character);
            //     if (!fastForwardToken.IsCancellationRequested && Delay != 0)
            //         yield return new WaitForSeconds(Delay);
            // }
            //
            // _lastPrintedText += _format.CloseTagger;
        }
    }
}