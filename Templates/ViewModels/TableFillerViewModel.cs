using Incas.Core.ViewModels;
using Incas.Templates.Models;
using System.Data;

namespace Incas.Templates.ViewModels
{
    internal class TableFillerViewModel : BaseViewModel
    {
        private DataTable _data;
        public TableFillerViewModel(Tag t)
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
            get => this._data;
            set
            {
                this._data = value;
                this.OnPropertyChanged(nameof(this.Grid));
            }
        }
    }
}
