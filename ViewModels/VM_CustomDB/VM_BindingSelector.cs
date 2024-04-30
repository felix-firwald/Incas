using Common;
using Incubator_2.Models;
using System.Collections.Generic;
using System.Windows;

namespace Incubator_2.ViewModels.VM_CustomDB
{
    public class VM_BindingSelector : VM_Base
    {
        private SDatabase _selectedDB;
        private string _selectedTable;
        private string _selectedField;
        private bool _dbEnable = true;
        private bool _tableEnable = true;
        public VM_BindingSelector()
        {

        }
        public Visibility TopPanelVisibility
        {
            get
            {
                if (!this._dbEnable && !this._tableEnable)
                {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }
        }
        public bool DatabaseSelectionEnable
        {
            get
            {
                return this._dbEnable;
            }
            set
            {
                this._dbEnable = value;
                this.OnPropertyChanged(nameof(this.DatabaseSelectionEnable));
                this.OnPropertyChanged(nameof(this.TopPanelVisibility));
            }
        }
        public bool TableSelectionEnable
        {
            get
            {
                return this._tableEnable;
            }
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
            get
            {
                return this._selectedDB;
            }
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
            get
            {
                return this._selectedTable;
            }
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
            get
            {
                return this._selectedField;
            }
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
