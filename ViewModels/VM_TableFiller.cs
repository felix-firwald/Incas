using Models;
using System.Data;

namespace Incubator_2.ViewModels
{
    internal class VM_TableFiller : VM_Base
    {
        private DataTable _data;
        public VM_TableFiller(Tag t)
        {
            this._data = new DataTable();
            this.MakeColumns(t.value);
        }
        private void MakeColumns(string columns)
        {
            foreach (string c in columns.Split(';'))
            {
                this.Grid.Columns.Add(c);
            }
        }
        public DataTable Grid
        {
            get { return this._data; }
            set
            {
                this._data = value;
                this.OnPropertyChanged(nameof(this.Grid));
            }
        }
    }
}
