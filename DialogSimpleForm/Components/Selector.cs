using Incas.Core.Classes;
using System.Collections.Generic;
using System.Linq;

namespace Incas.DialogSimpleForm.Components
{
    public class Selector
    {
        public Selector(Dictionary<object, string> pairs)
        {
            this.Pairs = pairs;
        }
        public Dictionary<object, string> Pairs { get; set; }
        public List<string> VisibleItems => this.Pairs.Values.ToList();
        public object SelectedObject { get; private set; }
        public string SelectedValue => this.SelectedObject is not null ? this.Pairs[this.SelectedObject] : "";
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
            try
            {
                if (this.Pairs.ContainsKey(selection))
                {
                    this.SelectedObject = selection;
                }            
            }
            catch (System.Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }
        }
        public void SetSelectionByIndex(int index)
        {
            this.SelectedObject = this.Pairs.Keys.ElementAt(index);
        }
    }
}
