using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Core.Classes
{
    public class ComboSelector
    {
        public ComboSelector(Dictionary<object, string> pairs)
        {
            this.Pairs = pairs;
        }
        public Dictionary<object, string> Pairs { get; private set; }
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
        public void SetSelection(string selection)
        {
            this.SelectedObject = this.Pairs.FirstOrDefault(x => x.Value == selection).Key;
        }
    }
}
