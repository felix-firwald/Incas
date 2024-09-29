using Incas.Core.ViewModels;
using Incas.Templates.Models;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace Incas.Templates.ViewModels
{
    internal class TableFillerViewModel : BaseViewModel
    {
        private DataTable _data;
        private bool showForm = false;
        public TableFillerViewModel(Objects.Models.Field t)
        {
            this._data = new DataTable();
            this.MakeColumns(t.Value.ToString());
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
        public bool ShowForm
        {
            get => this.showForm;
            set
            {
                this.showForm = value;
                this.OnPropertyChanged(nameof(this.ShowForm));
                //this.OnPropertyChanged(nameof(this.FormVisibility));
            }
        }
        //public Visibility FormVisibility
        //{
        //    get
        //    {
        //        switch(this._mode)
        //        {
        //            case TableFillerMode.GridOnly:
        //            default:
        //                return Visibility.Collapsed;
        //            case TableFillerMode.GridAndForm:
        //                return Visibility.Visible;
        //        }
        //    }
        //    set
        //    {
        //        this.OnPropertyChanged(nameof(this.FormVisibility));
        //    }
        //}
    }
}
