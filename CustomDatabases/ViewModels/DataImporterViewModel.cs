using Incas.Core.ViewModels;
using System.Data;

namespace Incas.CustomDatabases.ViewModels
{
    public class DataImporterViewModel : BaseViewModel
    {
        private DataTable _table;

        public DataImporterViewModel(DataTable dt)
        {
            this.Table = dt;
        }
        public DataTable Table
        {
            get => this._table;
            set
            {
                this._table = value;
                this.OnPropertyChanged(nameof(this.Table));
            }
        }
    }
}
