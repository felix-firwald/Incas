using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                OnPropertyChanged(nameof(DataTable));
            }
        }
    }
}
