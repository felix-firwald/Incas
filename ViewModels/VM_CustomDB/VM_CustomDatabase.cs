using Common;
using Incubator_2.Common;
using Incubator_2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Incubator_2.ViewModels.VM_CustomDB
{
    class VM_CustomDatabase : VM_Base
    {
        private string _selectedTable = "";
        private DataRow _selectedRow;
        private DataGridSelectionUnit _selectionUnit = DataGridSelectionUnit.FullRow;
        CustomTable requester = new();
        public VM_CustomDatabase() { }

        public bool CanUserEditTable
        {
            get
            {
                using (Parameter p = new())
                {
                    return !p.Exists(ParameterType.RESTRICT_EDIT_TABLE, _selectedTable, ProgramState.CurrentUser.id.ToString());
                }
            }
        }

        public Visibility EditingVisibility
        {
            get
            {
                if (SelectionUnit == DataGridSelectionUnit.FullRow)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public DataGridSelectionUnit SelectionUnit
        {
            get
            {
                return _selectionUnit;
            }
            set
            {
                if (_selectionUnit != value)
                {
                    _selectionUnit = value;
                    OnPropertyChanged(nameof(SelectionUnit));
                    OnPropertyChanged(nameof(EditingVisibility));
                }
            }
        }
        public DataTable Table
        {
            get
            {
                if (_selectedTable != "")
                {
                    return requester.GetTable(SelectedTable, SelectedDatabase.path);
                }
                else
                {
                    return new();
                }
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
        public SDatabase _selectedDatabase;
        public SDatabase SelectedDatabase
        {
            get
            {
                return _selectedDatabase;
            }
            set
            {
                _selectedDatabase = value;
                OnPropertyChanged(nameof(SelectedDatabase));
                OnPropertyChanged(nameof(Tables));
            }
        }
        public List<string> Tables
        {
            get
            {
                return requester.GetTablesList(SelectedDatabase.path);
            }
            set
            {
                OnPropertyChanged(nameof(Tables));
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
                OnPropertyChanged(nameof(Table));
                OnPropertyChanged(nameof(CanUserEditTable));
            }
        }
        
        public DataRow SelectedRow
        {
            get
            {
                return _selectedRow;
            }
            set
            {
                _selectedRow = value;
                OnPropertyChanged(nameof(SelectedRow));
            }
        }
        public List<FieldCreator> GetTableDefinition()
        {
            return requester.GetTableDefinition(SelectedTable, SelectedDatabase.path);
        }
        #region other
        public string GetPK()
        {
            return requester.GetPKField(SelectedTable, SelectedDatabase.path);
        }
        public void RefreshTable()
        {
            OnPropertyChanged(nameof(Databases));
            OnPropertyChanged(nameof(Tables));
            OnPropertyChanged(nameof(Table));
        }
        public void SwitchSelectionUnit()
        {
            if (_selectionUnit == DataGridSelectionUnit.Cell)
            {
                SelectionUnit = DataGridSelectionUnit.FullRow;
            }
            else
            {
                SelectionUnit = DataGridSelectionUnit.Cell;
            }
        }
        #endregion
    }
}
