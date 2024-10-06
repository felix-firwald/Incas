using System.Collections.Generic;
using System.Linq;

namespace Incas.Core.Classes
{
    public class Selector
    {
        public Selector(Dictionary<object, string> pairs)
        {
            this.Pairs = pairs;
        }
        public Dictionary<object, string> Pairs { get; set; }
        public List<string> VisibleItems
        {
            get
            {
                return this.Pairs.Values.ToList();
            }
        }
        public object SelectedObject { get; private set; }
        public string SelectedValue
        {
            get
            {
                if (this.SelectedObject is not null)
                {
                    return this.Pairs[this.SelectedObject];
                }
                return "";
            }
        }
        public void SetSelectionByValue(string selection)
        {
            this.SelectedObject = this.Pairs.FirstOrDefault(x => x.Value == selection).Key;
        }
        /// <summary>
        /// НЕ РАБОТАЕТ!
        /// </summary>
        /// <param name="selection"></param>
        public void SetSelection(object selection)
        {
            this.SelectedObject = selection;
            //foreach (KeyValuePair<object, string> item in this.Pairs)
            //{
            //    if (item.Key == selection)
            //    {
            //        this.SelectedObject = selection;
            //        return;
            //    } 
            //}
            //DialogsManager.ShowInfoDialog("Не найдено!");
        }
        public void SetSelectionByIndex(int index)
        {
            this.SelectedObject = this.Pairs.Keys.ElementAt(index);
        }
    }
}
