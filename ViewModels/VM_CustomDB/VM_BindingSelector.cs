using Common;
using Incubator_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
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
                if (!_dbEnable && !_tableEnable)
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
                return _dbEnable;
            }
            set
            {
                _dbEnable = value;
                OnPropertyChanged(nameof(DatabaseSelectionEnable));
                OnPropertyChanged(nameof(TopPanelVisibility));
            }
        }
        public bool TableSelectionEnable
        {
            get
            {
                return _tableEnable;
            }
            set
            {
                _tableEnable = value;
                OnPropertyChanged(nameof(TableSelectionEnable));
                OnPropertyChanged(nameof(TopPanelVisibility));
            }
        }

        public List<SDatabase> Databases
        {
            get
            {
                using (Database db = new())
                {
                    return db.GetActualDatabases();
                }
            }
        }
        public void SetSelectedDatabase(string path)
        {
            foreach (SDatabase db in Databases)
            {
                if (db.path == path)
                {
                    SelectedDatabase = db;
                }
            }
        }
        public SDatabase SelectedDatabase
        {
            get
            {
                return _selectedDB;
            }
            set
            {
                _selectedDB = value;
                OnPropertyChanged(nameof(SelectedDatabase));
                OnPropertyChanged(nameof(Tables));
            }
        }
        public List<string> Tables
        {
            get
            {
                using (CustomTable ct = new())
                {
                    return ct.GetTablesList(SelectedDatabase.path, TableType.All);
                }
            }
        }
        public string SelectedTable
        {
            get
            {
                return _selectedTable;
            }
            set
            {
                _selectedTable = value;
                OnPropertyChanged(nameof(SelectedTable));
                OnPropertyChanged(nameof(Fields));
            }
        }
        public List<string> Fields
        {
            get
            {
                using (CustomTable ct = new())
                {
                    return ct.GetTableFieldsSimple(SelectedTable, SelectedDatabase.path);
                }
            }
        }
        public string SelectedField
        {
            get
            {
                return _selectedField;
            }
            set
            {
                _selectedField = value;
                OnPropertyChanged(nameof(SelectedField));
            }
        }
        public bool ValidateContent()
        {
            if (SelectedField == null)
            {
                ProgramState.ShowExclamationDialog("Поле не выбрано!", "Действие невозможно");
                return false;
            }
            return true;
        }
    }
}
