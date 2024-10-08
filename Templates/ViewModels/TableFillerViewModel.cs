using Incas.Core.Classes;
using Incas.Core.ViewModels;
using Incas.Objects.Components;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Documents;

namespace Incas.Templates.ViewModels
{
    internal class TableFillerViewModel : BaseViewModel
    {
        private DataTable _data;
        public TableFieldData TableDefinition;
        public TableFillerViewModel(Objects.Models.Field t)
        {
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
        }

        private void MakeColumns()
        {
            this.Grid = new();
            foreach (TableFieldColumnData tf in this.TableDefinition.Columns)
            {
                DataColumn dc = new(tf.Name)
                {
                    DefaultValue = tf.Value
                };
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
        public void RemoveSelectedRow()
        {
            this.Grid.Rows.Remove(this.Grid.Rows[this.SelectedRow]);
            this.OnPropertyChanged(nameof(this.Grid));
        }
        public void MoveUpSelectedRow()
        {
            if (this.selected == -1)
            {
                return;
            }
            DataRow dr = this.Grid.Rows[this.SelectedRow];
            int position = this.SelectedRow;
            if (position > 0)
            {
                this.Grid.Rows.Remove(dr);
                this.Grid.Rows.InsertAt(dr, position - 1);
               
                this.OnPropertyChanged(nameof(this.Grid));
            }          
        }
        public void MoveDownSelectedRow()
        {
            if (this.selected == -1)
            {
                return;
            }
            DataRow dr = this.Grid.Rows[this.SelectedRow];
            int position = this.SelectedRow;
            if (position < this.Grid.Rows.Count)
            {
                this.Grid.Rows.Remove(dr);
                this.Grid.Rows.InsertAt(dr, position + 1);
                this.OnPropertyChanged(nameof(this.Grid));
            }
        }
    }
}
