namespace Doublsb.Dialog
{
    using System.Collections.Generic;

    public class DialogSelect
    {
        private List<DialogSelectItem> ItemList;

        public DialogSelect()
        {
            ItemList = new List<DialogSelectItem>();
        }

        public int Count
        {
            get => ItemList.Count;
        }

        public DialogSelectItem GetByIndex(int index)
        {
            return ItemList[index];
        }

        public List<DialogSelectItem> Get_List()
        {
            return ItemList;
        }

        public string Get_Value(string Key)
        {
            return ItemList.Find((var) => var.isSameKey(Key)).Value;
        }

        public void Clear()
        {
            ItemList.Clear();
        }

        public void Add(string Key, string Value)
        {
            ItemList.Add(new DialogSelectItem(Key, Value));
        }

        public void Remove(string Key)
        {
            var item = ItemList.Find((var) => var.isSameKey(Key));

            if (item != null) ItemList.Remove(item);
        }
    }
}