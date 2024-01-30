using Common;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using Incubator_2.Common;
using Incubator_2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.RightsManagement;
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
        private string _columnFilter = "";
        private string _searchText = "";
        private DataTable _dataTable = new();
        private bool isTableNeedUpdate = true;
        private DataGridSelectionUnit _selectionUnit = DataGridSelectionUnit.FullRow;
        private List<SCommand> _commands = new List<SCommand>();
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
        public Visibility CreateCommandVisibility
        {
            get
            {
                if (Permission.IsUserHavePermission(PermissionGroup.Moderator))
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        public Visibility TablesVisibility
        {
            get
            {
                if (SelectedDatabase.id == 0)
                {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
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
        public bool EditingEnabled
        {
            get
            {
                if (SelectionUnit == DataGridSelectionUnit.FullRow)
                {
                    return true;
                }
                else
                {
                    return false;
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
                    OnPropertyChanged(nameof(EditingEnabled));
                }
            }
        }


        public string CustomViewRequest;

        
        public DataTable Table
        {
            get
            {
                if (!string.IsNullOrEmpty(_selectedTable))
                {
                    
                    return _dataTable;
                }
                else
                {
                    return new();
                }
            }
        }
        public List<string> Columns
        {
            get
            {
                List<string> columns = new();
                foreach (DataColumn col in _dataTable.Columns)
                {
                    columns.Add(col.ColumnName);
                }
                if (columns.Count > 0)
                {
                    ColumnFilter = columns[0];
                }
                return columns;
            }
        }
        public string ColumnFilter
        {
            get
            {
                return _columnFilter;
            }
            set
            {
                _columnFilter = value;
                OnPropertyChanged(nameof(ColumnFilter));
            }
        }
        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                OnPropertyChanged(nameof(Table));
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
                OnPropertyChanged(nameof(TablesVisibility));
                SelectedTable = Tables.FirstOrDefault();
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
                CustomViewRequest = null;
                //OnPropertyChanged(nameof(Table));
                UpdateTable();
                OnPropertyChanged(nameof(CanUserEditTable));
                
                UpdateListOfCommands();
                OnPropertyChanged(nameof(ReadCommands));
                OnPropertyChanged(nameof(UpdateCommands));
                OnPropertyChanged(nameof(Columns));
            }
        }
        public void UpdateTable()
        {
            _dataTable = requester.GetTable(SelectedTable, SelectedDatabase.path, CustomViewRequest);
            OnPropertyChanged(nameof(Table));
        }
        private void UpdateListOfCommands()
        {
            using (Models.Command c = new())
            {
                Commands = c.GetCommandsOfTable(SelectedDatabase.path, SelectedTable);
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
        public List<SCommand> Commands
        {
            get
            {
                return _commands;
            }
            set
            {
                _commands = value;
                OnPropertyChanged(nameof(Commands));
                OnPropertyChanged(nameof(ReadCommands));
                OnPropertyChanged(nameof(UpdateCommands));
            }
        }
        public List<SCommand> ReadCommands
        {
            get
            {
                return _commands.Where(x => x.type == Models.CommandType.Read).ToList();
            }
        }
        public List<SCommand> UpdateCommands
        {
            get
            {
                return _commands.Where(x => x.type == Models.CommandType.Update).ToList();
            }
        }
        public SCommand GetCommand(int id)
        {
            foreach (SCommand command in Commands)
            {
                if (command.id == id)
                {
                    return command;
                }
            }
            return new();
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
        public void ClearTableFromCustomView()
        {           
            SearchText = string.Empty;
            SelectedTable = SelectedTable;
        }
        public void RefreshCommands()
        {
            UpdateListOfCommands();
            OnPropertyChanged(nameof(Commands));
            OnPropertyChanged(nameof(ReadCommands));
            OnPropertyChanged(nameof(UpdateCommands));
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
        public void CustomUpdateRequest(string query)
        {
            requester.CustomRequest(SelectedDatabase.path, query);
            this.RefreshTable();
        }
        
        #endregion
    }
}
