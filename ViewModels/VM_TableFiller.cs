using Models;
using System.Data;

namespace Incubator_2.ViewModels
{
    class VM_TableFiller : VM_Base
    {
        private DataTable _data;
        public VM_TableFiller(Tag t)
        {
            _data = new DataTable();
            MakeColumns(t.value);
        }
        private void MakeColumns(string columns)
        {
            foreach (string c in columns.Split(';'))
            {
                Grid.Columns.Add(c);
            }
        }
        public DataTable Grid
        {
            get { return _data; }
            set
            {
                _data = value;
                OnPropertyChanged(nameof(Grid));
            }
        }
    }
}
