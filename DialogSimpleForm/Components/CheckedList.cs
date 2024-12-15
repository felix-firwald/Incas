using System.Collections.Generic;

namespace Incas.DialogSimpleForm.Components
{
    public struct CheckedItem
    {
        public object Target { get; set; }
        public string Name { get; set; }
    }
    public class CheckedList
    {
        public Dictionary<CheckedItem, bool> Pairs { get; set; }
        public CheckedList(Dictionary<CheckedItem, bool> Pairs)
        {
            this.Pairs = Pairs;
        }
        public CheckedList(List<CheckedItem> items, bool setAll = false)
        {
            this.Pairs = new();
            foreach (CheckedItem ci in items)
            {
                this.Pairs.Add(ci, setAll);
            }
        }
    }
}
