namespace Doublsb.Dialog
{
    using UnityEngine;
    using UnityEngine.UI;

    internal class BasicDialogView : MonoBehaviour, IDialogView
    {
        [SerializeField]
        private GameObject textWindow;
        
        [SerializeField]
        private Text textComponent;

        public string Text
        {
            get => textComponent.text;
            set => textComponent.text = value;
        }

        public void SetActive(bool active)
        {
            textWindow.SetActive(active);
        }
    }
}