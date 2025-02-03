using Incas.Core.Classes;
using Incas.Core.ViewModels;
using Incas.Objects.Components;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Input;

namespace Incas.Rendering.ViewModels
{
    internal class TableFillerViewModel : BaseViewModel
    {
        private DataTable _data;
        public TableFieldData TableDefinition;
        public TableFillerViewModel(Field t)
        {
            this.TableName = t.VisibleName;
            try
            {
                this.TableDefinition = JsonConvert.DeserializeObject<TableFieldData>(t.Value);
                this.MakeColumns();
            }
            catch
            {
                DialogsManager.ShowErrorDialog("Не удалось получить определение таблицы.");
            }
            this._data = new DataTable();
            this.MakeColumns();
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
            foreach (TableFieldColumnData tf in this.TableDefinition.Columns)
            {
                DataColumn dc = new(tf.Name);
                if (tf.FieldType == FieldType.Variable)
                {
                    dc.DefaultValue = tf.Value;
                }
                this.Grid.Columns.Add(dc);
            }
            this.OnPropertyChanged(nameof(this.Grid));
        }
        public void ApplyData(DataTable data)
        {
            this.Grid.Clear();
            foreach (DataRow dc in data.Rows)
            {
                this.Grid.ImportRow(dc);
            }
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
        public void RemoveSelectedRow()
        {
            int counter = this.SelectedRow;
            this.Grid.Rows.Remove(this.Grid.Rows[counter]);
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
            TableFieldColumnData target = null;
            foreach (TableFieldColumnData tf in this.TableDefinition.Columns)
            {
                if (tf.Name == targetColumn)
                {
                    target = tf;
                    break;
                }
            }
            List<string> values = new();
            switch (target.FieldType)
            {
                case FieldType.Variable:
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
            foreach (TableFieldColumnData tf in this.TableDefinition.Columns)
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
