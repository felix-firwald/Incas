using Incas.Core.Models;
using Incas.Core.ViewModels;
using Incas.Objects.Models;
using Incas.Templates.Components;
using System.Collections.Generic;
using System.Windows;

namespace Incas.Objects.ViewModels
{
    public class FieldViewModel : BaseViewModel
    {
        public Field Source;
        public FieldViewModel(Field field)
        {
            this.Source = field;
        }
        public FieldViewModel()
        {
            this.Source = new();
        }
        public string VisibleName
        {
            get => this.Source.VisibleName;
            set
            {
                this.Source.VisibleName = value;
                this.OnPropertyChanged(nameof(this.VisibleName));
            }
        }
        public int OrderNumber
        {
            get => this.Source.OrderNumber;
            set
            {
                if (value is >= 0 and <= 50)
                {
                    this.Source.OrderNumber = value;
                    this.OnPropertyChanged(nameof(this.OrderNumber));
                }
            }
        }
        public string SelectedValue
        {
            get => this.Source.Value;
            set
            {
                this.Source.Value = value;
                this.OnPropertyChanged(nameof(this.SelectedValue));
            }
        }
        public void IncrementOrder()
        {
            this.OrderNumber++;
        }
        public void DecrementOrder()
        {
            this.OrderNumber--;
        }

        public string NameOfTag
        {
            get => this.Source.Name;
            set
            {
                if (value != this.Source.Name)
                {
                    this.Source.Name = value;
                    this.OnPropertyChanged(nameof(this.NameOfTag));
                }
            }
        }

        public string TypeOfTagValue
        {
            get => this.SerializeToInput(this.Source.Type);
            set
            {
                this.Source.Type = this.SerializeFromInput(value);
                this.OnPropertyChanged(nameof(this.TypeOfTagValue));
            }
        }

        // может лучше по selected index?
        #region Not Standart Properties
        public TagType SerializeFromInput(string val)
        {
            switch (val)
            {
                case "0":
                default:
                    return TagType.Variable;
                case "1":
                    return TagType.Text;
                case "2":
                    return TagType.LocalEnumeration;
                case "3":
                    return TagType.GlobalEnumeration;
                case "4":
                    return TagType.Relation;
                case "5":
                    return TagType.Date;
                case "6":
                    return TagType.Number;
                case "7":
                    return TagType.LocalConstant;
                case "8":
                    return TagType.GlobalConstant;
                case "9":
                    return TagType.HiddenField;
                case "10":
                    return TagType.Generator;
                case "11":
                    return TagType.Macrogenerator;
                case "12":
                    return TagType.Table;
            }
        }
        public string SerializeToInput(TagType tot)
        {
            return tot switch
            {
                TagType.Text => "1",
                TagType.LocalEnumeration => "2",
                TagType.GlobalEnumeration => "3",
                TagType.Relation => "4",
                TagType.Date => "5",
                TagType.Number => "6",
                TagType.LocalConstant => "7",
                TagType.GlobalConstant => "8",
                TagType.HiddenField => "9",
                TagType.Generator => "10",
                TagType.Macrogenerator => "11",
                TagType.Table => "12",
                _ => "0",
            };
        }
    }
}
#endregion