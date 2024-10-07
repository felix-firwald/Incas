using Incas.Core.Classes;
using Incas.Core.ViewModels;
using Incas.Objects.Components;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
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
    }
}
