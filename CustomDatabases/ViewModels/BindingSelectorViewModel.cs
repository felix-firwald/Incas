using Common;
using Incas.Core.ViewModels;
using Incubator_2.Models;
using System.Collections.Generic;
using System.Windows;

namespace Incas.CustomDatabases.ViewModels
{
    public class BindingSelectorViewModel : BaseViewModel
    {
        private SDatabase _selectedDB;
        private string _selectedTable;
        private string _selectedField;
        private bool _dbEnable = true;
        private bool _tableEnable = true;
        public BindingSelectorViewModel()
        {

        }
        public Visibility TopPanelVisibility => !this._dbEnable && !this._tableEnable ? Visibility.Collapsed : Visibility.Visible;
        public bool DatabaseSelectionEnable
        {
            get => this._dbEnable;
            set
            {
                this._dbEnable = value;
                this.OnPropertyChanged(nameof(this.DatabaseSelectionEnable));
                this.OnPropertyChanged(nameof(this.TopPanelVisibility));
            }
        }
        public bool TableSelectionEnable
        {
            get => this._tableEnable;
            set
            {
                this._tableEnable = value;
                this.OnPropertyChanged(nameof(this.TableSelectionEnable));
                this.OnPropertyChanged(nameof(this.TopPanelVisibility));
            }
        }

        public List<SDatabase> Databases
        {
            get
            {
                using Database db = new();
                return db.GetActualDatabases();
            }
        }
        public void SetSelectedDatabase(string path)
        {
            foreach (SDatabase db in this.Databases)
            {
                if (db.path == path)
                {
                    this.SelectedDatabase = db;
                }
            }
        }
        public SDatabase SelectedDatabase
        {
            get => this._selectedDB;
            set
            {
                this._selectedDB = value;
                this.OnPropertyChanged(nameof(this.SelectedDatabase));
                this.OnPropertyChanged(nameof(this.Tables));
            }
        }
        public List<string> Tables
        {
            get
            {
                using CustomTable ct = new();
                return ct.GetTablesList(this.SelectedDatabase.path, TableType.All);
            }
        }
        public string SelectedTable
        {
            get => this._selectedTable;
            set
            {
                this._selectedTable = value;
                this.OnPropertyChanged(nameof(this.SelectedTable));
                this.OnPropertyChanged(nameof(this.Fields));
            }
        }
        public List<string> Fields
        {
            get
            {
                using CustomTable ct = new();
                return ct.GetTableFieldsSimple(this.SelectedTable, this.SelectedDatabase.path);
            }
        }
        public string SelectedField
        {
            get => this._selectedField;
            set
            {
                this._selectedField = value;
                this.OnPropertyChanged(nameof(this.SelectedField));
            }
        }
        public bool ValidateContent()
        {
            if (this.SelectedField == null)
            {
                ProgramState.ShowExclamationDialog("Поле не выбрано!", "Действие невозможно");
                return false;
            }
            return true;
        }
    }
}
