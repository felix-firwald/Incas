using Incubator_2.Models;
using System.Data;

namespace Incubator_2.ViewModels.VM_CustomDB
{
    public class VM_DatabaseSelection : VM_Base
    {
        CustomTable requester = new();
        private string _database;
        private string _table;
        public VM_DatabaseSelection(string database, string table)
        {
            _database = database;
            _table = table;
        }
        public DataTable Table
        {
            get
            {
                return requester.GetTable(_table, _database, "");

            }
            set
            {
                OnPropertyChanged(nameof(Table));
            }
        }
    }
}
