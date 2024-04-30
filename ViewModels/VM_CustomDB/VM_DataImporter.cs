using System.Data;

namespace Incubator_2.ViewModels.VM_CustomDB
{
    public class VM_DataImporter : VM_Base
    {
        private DataTable _table;

        public VM_DataImporter(DataTable dt)
        {
            this.Table = dt;
        }
        public DataTable Table
        {
            get
            {
                return this._table;
            }
            set
            {
                this._table = value;
                this.OnPropertyChanged(nameof(this.Table));
            }
        }
    }
}
