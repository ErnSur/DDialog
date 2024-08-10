namespace QuickEye.PeeDialog
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UIElements;

    [Serializable]
    public class PrinterTmp : BasicPrinter
    {
        [SerializeField]
        private GameObject TextWindow;

        [SerializeField]
        private TMP_Text TextComponent;

        public override string Text
        {
            get => TextComponent.text;
            set => TextComponent.text = value;
        }

        public override void SetActive(bool active)
        {
            TextWindow.SetActive(active);
        }
    }
    
    
    [Serializable]
    public class PrinterUitk : BasicPrinter
    {
        [NonSerialized]
        public VisualElement TextWindow;
        
        [NonSerialized]
        public TextElement TextElement;

        public override string Text
        {
            get => TextElement.text;
            set => TextElement.text = value;
        }

        public override void SetActive(bool active)
        {
            if (TextWindow is null)
                return;

            TextWindow.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}