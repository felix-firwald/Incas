using Incas.Core.ViewModels;
using Incas.CustomDatabases.Models;
using System.Data;

namespace Incas.CustomDatabases.ViewModels
{
    public class DatabaseSelectionViewModel : BaseViewModel
    {
        private CustomTable requester = new();
        private string _database;
        private string _table;
        public DatabaseSelectionViewModel(string database, string table)
        {
            this._database = database;
            this._table = table;
        }
        public DataTable Table
        {
            get => this.requester.GetTable(this._table, this._database, "");
            set => this.OnPropertyChanged(nameof(this.Table));
        }
    }
}
