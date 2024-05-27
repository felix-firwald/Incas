using Common;
using Incas.Core.ViewModels;
using Incubator_2.Common;
using Incubator_2.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Incubator_2.ViewModels.VM_CustomDB
{
    internal class VM_CustomDatabase : BaseViewModel
    {
        private string _selectedTable = "";
        private DataRow _selectedRow;
        private string _columnFilter = "";
        private string _searchText = "";
        private DataTable _dataTable = new();
        private DataGridSelectionUnit _selectionUnit = DataGridSelectionUnit.FullRow;
        private List<SCommand> _commands = new List<SCommand>();
        private CustomTable requester = new();

        public VM_CustomDatabase() { }

        public bool CanUserEditTable
        {
            get
            {
                return true;
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
                if (this.SelectedDatabase.id == 0)
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
                if (this.SelectionUnit == DataGridSelectionUnit.FullRow)
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
                if (this.SelectionUnit == DataGridSelectionUnit.FullRow)
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
                return this._selectionUnit;
            }
            set
            {
                if (this._selectionUnit != value)
                {
                    this._selectionUnit = value;
                    this.OnPropertyChanged(nameof(this.SelectionUnit));
                    this.OnPropertyChanged(nameof(this.EditingVisibility));
                    this.OnPropertyChanged(nameof(this.EditingEnabled));
                }
            }
        }


        public string CustomViewRequest;


        public DataTable Table
        {
            get
            {
                if (!string.IsNullOrEmpty(this._selectedTable))
                {
                    return this._dataTable;
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
                foreach (DataColumn col in this._dataTable.Columns)
                {
                    columns.Add(col.ColumnName);
                }
                if (columns.Count > 0)
                {
                    this.ColumnFilter = columns[0];
                }
                return columns;
            }
        }
        public string ColumnFilter
        {
            get
            {
                return this._columnFilter;
            }
            set
            {
                this._columnFilter = value;
                this.OnPropertyChanged(nameof(this.ColumnFilter));
            }
        }
        public string SearchText
        {
            get
            {
                return this._searchText;
            }
            set
            {
                this._searchText = value;
                this.OnPropertyChanged(nameof(this.SearchText));
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
        public SDatabase _selectedDatabase;
        public SDatabase SelectedDatabase
        {
            get
            {
                return this._selectedDatabase;
            }
            set
            {
                this._selectedDatabase = value;
                this.OnPropertyChanged(nameof(this.SelectedDatabase));
                this.OnPropertyChanged(nameof(this.Tables));
                this.OnPropertyChanged(nameof(this.TablesVisibility));
                this.SelectedTable = this.Tables.FirstOrDefault();
            }
        }
        public List<string> Tables
        {
            get
            {
                return this.requester.GetTablesList(this.SelectedDatabase.path);
            }
            set
            {
                this.OnPropertyChanged(nameof(this.Tables));
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
                this.CustomViewRequest = null;
                this.UpdateTable();
                this.OnPropertyChanged(nameof(this.CanUserEditTable));
                this.UpdateListOfCommands();
                this.OnPropertyChanged(nameof(this.Columns));
            }
        }
        public void UpdateTable()
        {
            if (!string.IsNullOrEmpty(this.SelectedTable))
            {
                ProgramState.ShowWaitCursor();
                this._dataTable = this.requester.GetTable(this.SelectedTable, this.SelectedDatabase.path, this.CustomViewRequest);
                this.OnPropertyChanged(nameof(this.Table));
                ProgramState.ShowWaitCursor(false);
            }
        }
        private void UpdateListOfCommands()
        {
            using (Models.Command c = new())
            {
                this.Commands = c.GetCommandsOfTable(this.SelectedDatabase.path, this.SelectedTable);
            }
            this.OnPropertyChanged(nameof(this.ReadCommands));
            this.OnPropertyChanged(nameof(this.UpdateCommands));
        }

        public DataRow SelectedRow
        {
            get
            {
                return this._selectedRow;
            }
            set
            {
                this._selectedRow = value;
                this.OnPropertyChanged(nameof(this.SelectedRow));
            }
        }
        public List<SCommand> Commands
        {
            get
            {
                return this._commands;
            }
            set
            {
                this._commands = value;
                this.OnPropertyChanged(nameof(this.Commands));
                this.OnPropertyChanged(nameof(this.ReadCommands));
                this.OnPropertyChanged(nameof(this.UpdateCommands));
            }
        }
        public List<SCommand> ReadCommands
        {
            get
            {
                return this._commands.Where(x => x.type == Models.CommandType.Read).ToList();
            }
        }
        public List<SCommand> UpdateCommands
        {
            get
            {
                return this._commands.Where(x => x.type == Models.CommandType.Update).ToList();
            }
        }
        public SCommand GetCommand(int id)
        {
            foreach (SCommand command in this.Commands)
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
            return this.requester.GetTableDefinition(this.SelectedTable, this.SelectedDatabase.path);
        }
        #region other
        public string GetPK()
        {
            return this.requester.GetPKField(this.SelectedTable, this.SelectedDatabase.path);
        }
        public List<string> GetFieldsSimple()
        {
            return this.requester.GetTableFieldsSimple(this.SelectedTable, this.SelectedDatabase.path);
        }
        public void RefreshTable()
        {
            this.OnPropertyChanged(nameof(this.Databases));
            this.OnPropertyChanged(nameof(this.Tables));

            this.OnPropertyChanged(nameof(this.Table));
        }
        public void ClearTableFromCustomView()
        {
            this.SearchText = string.Empty;
            this.SelectedTable = this.SelectedTable;
        }
        public void RefreshCommands()
        {
            this.UpdateListOfCommands();
            this.OnPropertyChanged(nameof(this.Commands));
            this.OnPropertyChanged(nameof(this.ReadCommands));
            this.OnPropertyChanged(nameof(this.UpdateCommands));
        }
        public void SwitchSelectionUnit()
        {
            if (this._selectionUnit == DataGridSelectionUnit.Cell)
            {
                this.SelectionUnit = DataGridSelectionUnit.FullRow;
            }
            else
            {
                this.SelectionUnit = DataGridSelectionUnit.Cell;
            }
        }
        public void CustomUpdateRequest(string query)
        {
            this.requester.CustomRequest(this.SelectedDatabase.path, query);
            //this.RefreshTable();
            this.UpdateTable();
        }


        #endregion
    }
}
