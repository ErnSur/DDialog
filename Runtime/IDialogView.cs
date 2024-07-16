namespace Doublsb.Dialog
{
    public interface IDialogView
    {
        public string Text { get; set; }
        
        public void SetActive(bool active);
    }
}