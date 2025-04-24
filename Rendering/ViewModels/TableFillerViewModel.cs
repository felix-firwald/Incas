using Incas.Core.Classes;
using Incas.Core.ViewModels;
using Incas.Objects.ViewModels;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Incas.Rendering.ViewModels
{
    public class TableFillerViewModel : BaseViewModel
    {
        private DataTable _data;
        public Table TableDefinition;
        public TableFillerViewModel(Table t)
        {
            this.TableName = t.VisibleName;
            try
            {
                this.TableDefinition = t;
                this.configurations = new();
                foreach (Field field in t.Fields)
                {
                    this.Configurations.Add(field.Id, new());
                }
                //this.MakeColumns();
            }
            catch
            {
                DialogsManager.ShowErrorDialog("Не удалось получить определение таблицы.");
            }
            this._data = new DataTable();
            //this.MakeColumns();
            this.SetCommands();
        }
        #region Commands
        private void SetCommands()
        {
            this.InsertToRight = new Command(this.DoInsertToRight);
            this.InsertToLeft = new Command(this.DoInsertToLeft);
            this.InsertToTop = new Command(this.DoInsertToTop);
            this.InsertToBottom = new Command(this.DoInsertToBottom);
        }
        public ICommand InsertToRight { get; private set; }
        public ICommand InsertToLeft { get; private set; }
        public ICommand InsertToTop { get; private set; }
        public ICommand InsertToBottom { get; private set; }

        private bool isEnabled = true;
        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
            set
            {
                this.isEnabled = value;
                this.OnPropertyChanged(nameof(this.IsEnabled));
                this.OnPropertyChanged(nameof(this.IsReadOnly));
                this.OnPropertyChanged(nameof(this.EditVisibility));
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return !this.IsEnabled;
            }
        }
        private bool insertEnabled = true;
        public bool InsertEnabled
        {
            get
            {
                return this.insertEnabled;
            }
            set
            {
                this.insertEnabled = value;
                this.OnPropertyChanged(nameof(this.InsertEnabled));
            }
        }
        private bool removeEnabled = true;
        public bool RemoveEnabled
        {
            get
            {
                return this.removeEnabled;
            }
            set
            {
                this.removeEnabled = value;
                this.OnPropertyChanged(nameof(this.RemoveEnabled));
            }
        }

        public Visibility EditVisibility
        {
            get
            {
                return this.FromBool(this.IsEnabled);
            }
        }
        private Dictionary<Guid, ColumnConfiguration> configurations;
        public Dictionary<Guid, ColumnConfiguration> Configurations
        {
            get
            {
                return this.configurations;
            }
            set
            {
                this.configurations = value;
                this.OnPropertyChanged(nameof(this.Configurations));
            }
        }

        public void DoInsertToRight(object parameter)
        {
            
        }
        public void DoInsertToLeft(object parameter)
        {
            
        }
        public void DoInsertToTop(object parameter)
        {
            DialogsManager.ShowInfoDialog(this.SelectedItem);
            foreach (DataRow dr in this.Grid.Rows)
            {

            }            
        }
        public void DoInsertToBottom(object parameter)
        {
            
        }
        #endregion
        private void MakeColumns()
        {
            this.Grid = new();
            foreach (Field tf in this.TableDefinition.Fields)
            {
                DataColumn dc = new(tf.Name);
                if (tf.Type == FieldType.String)
                {
                    dc.DefaultValue = tf.Value;
                }
                dc.ExtendedProperties.Add(Helpers.TableColumnIdKey, tf.Id);
                this.Grid.Columns.Add(dc);
            }
            this.OnPropertyChanged(nameof(this.Grid));
        }
        public void ApplyData(DataTable data)
        {
            this.Grid = data;
        }
        
        public void AddRow()
        {
            this.Grid.Rows.Add();
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
        private string tableName;
        public string TableName
        {
            get
            {
                return this.tableName;
            }
            set
            {
                this.tableName = value;
                this.OnPropertyChanged(nameof(this.TableName));
            }
        }
        private int selected;
        public int SelectedRow
        {
            get
            {
                return this.selected;
            }
            set
            {
                this.selected = value;
                this.OnPropertyChanged(nameof(this.SelectedRow));
            }
        }
        private object selectedItem;
        public object SelectedItem
        {
            get
            {
                return this.selectedItem;
            }
            set
            {
                this.selectedItem = value;
                this.OnPropertyChanged(nameof(this.SelectedItem));
            }
        }
        private List<string> removed = new();
        public List<string> RemovedRows
        {
            get
            {
                return this.removed;
            }
        }
        public void RemoveSelectedRow()
        {
            int counter = this.SelectedRow;
            DataRow dr = this.Grid.Rows[counter];
            string id = dr[Helpers.IdField].ToString();
            if (!string.IsNullOrEmpty(id))
            {
                this.RemovedRows.Add(id);
            }
            this.Grid.Rows.Remove(dr);
            this.OnPropertyChanged(nameof(this.Grid));
            if (counter > 0)
            {
                counter--;
                this.SelectedRow = counter;
            }                     
        }
        public void MoveUpSelectedRow()
        {
            if (this.selected < 1)
            {
                return;
            }
            int position = this.SelectedRow - 1;
            DataRow newdata = this.Grid.NewRow();
            newdata.ItemArray = this.Grid.Rows[this.SelectedRow].ItemArray;
            this.Grid.Rows.RemoveAt(this.SelectedRow);
            this.Grid.Rows.InsertAt(newdata, position);
            this.Grid.AcceptChanges();
            this.SelectedRow = position;
        }
        public void MoveDownSelectedRow()
        {
            if (this.selected == -1)
            {
                return;
            }

            int position = this.SelectedRow + 1;           
            if (position < this.Grid.Rows.Count)
            {
                DataRow newdata = this.Grid.NewRow();
                newdata.ItemArray = this.Grid.Rows[this.SelectedRow].ItemArray;
                this.Grid.Rows.RemoveAt(this.SelectedRow);
                this.Grid.Rows.InsertAt(newdata, position);
                this.Grid.AcceptChanges();
                this.SelectedRow = position;
            }
        }
        public void CopyColumnValuesToAnother(string souceColumn, string targetColumn)
        {
            Field target = null;
            foreach (Field tf in this.TableDefinition.Fields)
            {
                if (tf.Name == targetColumn)
                {
                    target = tf;
                    break;
                }
            }
            List<string> values = new();
            switch (target.Type)
            {
                case FieldType.String:
                case FieldType.Integer:
                case FieldType.Float:
                case FieldType.Boolean:
                    foreach (DataRow row in this.Grid.Rows)
                    {
                        row[targetColumn] = row[souceColumn];
                    }
                    return;
                case FieldType.LocalEnumeration:
                    values = JsonConvert.DeserializeObject<List<string>>(target.Value);
                    break;
                case FieldType.GlobalEnumeration:
                    values = ProgramState.GetEnumeration(Guid.Parse(target.Value));
                    break;
            }
            foreach (DataRow row in this.Grid.Rows)
            {
                if (values.Contains(row[souceColumn]))
                {
                    row[targetColumn] = row[souceColumn];
                }             
            }
        }
        private string LastSort = "";
        public void SortByColumn(string visiblename)
        {
            string name = "";
            foreach (Field tf in this.TableDefinition.Fields)
            {
                if (tf.VisibleName == visiblename)
                {
                    name = tf.Name;
                    break;
                }
            }
            DataView dv = this.Grid.DefaultView;
            if (this.LastSort == $"[{name}] ASC")
            {
                this.LastSort = $"[{name}] DESC";
            }
            else
            {
                this.LastSort = $"[{name}] ASC";
            }           
            dv.Sort = this.LastSort;
            this.Grid = dv.ToTable();
            this.Grid.AcceptChanges();
        }
        public void CopyValueToAllRows(string column, bool onlyEmptyOnes)
        {
            string value = this.Grid.Rows[this.SelectedRow][column].ToString();
            if (onlyEmptyOnes)
            {
                foreach (DataRow row in this.Grid.Rows)
                {
                    if (string.IsNullOrWhiteSpace(row[column].ToString()))
                    {
                        row[column] = value;
                    }                  
                }
            }
            else
            {
                foreach (DataRow row in this.Grid.Rows)
                {
                    row[column] = value;
                }
            }          
        }
    }
}
