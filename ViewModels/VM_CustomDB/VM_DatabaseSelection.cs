using Incas.Core.ViewModels;
using Incubator_2.Models;
using System.Data;

namespace Incubator_2.ViewModels.VM_CustomDB
{
    public class VM_DatabaseSelection : BaseViewModel
    {
        private CustomTable requester = new();
        private string _database;
        private string _table;
        public VM_DatabaseSelection(string database, string table)
        {
            this._database = database;
            this._table = table;
        }
        public DataTable Table
        {
            get
            {
                return this.requester.GetTable(this._table, this._database, "");

            }
            set
            {
                this.OnPropertyChanged(nameof(this.Table));
            }
        }
    }
}
