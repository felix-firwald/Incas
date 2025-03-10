using Incas.Core.ViewModels;
using Incas.Objects.Components;
using IncasEngine.Core.ExtensionMethods;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.FieldComponents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static IncasEngine.ObjectiveEngine.FieldComponents.TableFieldData;

namespace Incas.Objects.ViewModels
{
    public class TableSettingsViewModel : BaseViewModel
    {
        public TableFieldData Source { get; set; }
        public TableSettingsViewModel(TableFieldData source)
        {
            this.Source = source;
            this.Fields = new();
            if (this.Source.Columns is not null)
            {
                foreach (TableFieldColumnData field in this.Source.Columns)
                {
                    this.AddColumn(field);
                }
            }           
        }
        #region Commands
        
        #endregion
        public void AddColumn(TableFieldColumnData columnVM)
        {
            TableColumnViewModel col = new(columnVM);
            col.OnMoveDownRequested += this.Col_OnMoveDownRequested;
            col.OnMoveUpRequested += this.Col_OnMoveUpRequested;
            col.OnRemoveRequested += this.Col_OnRemoveRequested;
            this.Fields.Add(col);
        }

        private void Col_OnRemoveRequested(TableColumnViewModel vm)
        {
            this.Fields.Remove(vm);
        }

        private void Col_OnMoveUpRequested(TableColumnViewModel vm)
        {
            this.Fields.MoveUp(vm);
        }

        private void Col_OnMoveDownRequested(TableColumnViewModel vm)
        {
            this.Fields.MoveDown(vm);
        }

        private ObservableCollection<TableColumnViewModel> fields;
        public ObservableCollection<TableColumnViewModel> Fields
        {
            get
            {
                return this.fields;
            }
            set
            {
                this.fields = value;
                this.OnPropertyChanged(nameof(this.Fields));
            }
        }
        private static List<FieldType> fieldTypes = new()
        {
            FieldType.String,
            FieldType.Boolean,
            FieldType.Integer,
            FieldType.Float,
            FieldType.Date,
            FieldType.LocalEnumeration,
            FieldType.GlobalEnumeration,
            FieldType.Object
        };
        public List<FieldType> FieldTypes
        {
            get
            {
                return fieldTypes;
            }
        }
        public void Save()
        {
            this.Source.Columns = new();
            foreach (TableColumnViewModel vm in this.Fields)
            {
                this.Source.Columns.Add(vm.Source);
            }
        }
    }
}
