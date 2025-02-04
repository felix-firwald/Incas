using Incas.Core.ViewModels;
using Incas.Objects.Components;
using IncasEngine.ObjectiveEngine.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.ViewModels
{
    public class TableColumnViewModel : BaseViewModel
    {
        public TableFieldColumnData FieldData { get; set; }
        public TableColumnViewModel(TableFieldColumnData fieldData)
        {
            this.FieldData = fieldData;
        }
        public string VisibleName
        {
            get => this.FieldData.VisibleName;
            set
            {
                this.FieldData.VisibleName = value;
                this.OnPropertyChanged(nameof(this.VisibleName));
            }
        }
        public string NameOfField
        {
            get => this.FieldData.Name;
            set
            {
                if (value != this.FieldData.Name)
                {
                    this.FieldData.Name = value
                        .Replace(" ", "_")
                        .Replace(".", "_")
                        .Replace("$", "_");
                    this.OnPropertyChanged(nameof(this.NameOfField));
                }
            }
        }
        public string TypeOfFieldValue
        {
            get => this.SerializeToInput(this.FieldData.FieldType);
            set
            {
                this.FieldData.FieldType = this.SerializeFromInput(value);
                this.OnPropertyChanged(nameof(this.TypeOfFieldValue));
            }
        }

        // может лучше по selected index?
        #region Not Standart Properties
        public FieldType SerializeFromInput(string val)
        {
            switch (val)
            {
                case "0":
                default:
                    return FieldType.Variable;
                case "1":
                    return FieldType.LocalEnumeration;
                case "2":
                    return FieldType.GlobalEnumeration;
                case "3":
                    return FieldType.Relation;
                case "4":
                    return FieldType.Date;
                case "5":
                    return FieldType.Boolean;
                case "6":
                    return FieldType.Number;
            }
        }
        public string SerializeToInput(FieldType tot)
        {
            return tot switch
            {
                FieldType.Text => "1",
                FieldType.LocalEnumeration => "1",
                FieldType.GlobalEnumeration => "2",
                FieldType.Relation => "3",
                FieldType.Date => "4",
                FieldType.Boolean => "5",
                FieldType.Number => "6",
                _ => "0",
            };
        }
        #endregion
    }
}
