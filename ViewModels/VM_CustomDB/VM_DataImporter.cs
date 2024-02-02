using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.ViewModels.VM_CustomDB
{
    public class VM_DataImporter : VM_Base
    {
        private DataTable _table;
        
        public VM_DataImporter(DataTable dt)
        {
            Table = dt;
        }
        public DataTable Table
        {
            get
            {
                return _table;
            }
            set
            {
                _table = value;
                OnPropertyChanged(nameof(Table));
            }
        }
    }
}
