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
        private Field source;
        public FieldViewModel(Field field)
        {
            this.source = field;
        }
        public FieldViewModel()
        {
            this.source = new();
        }
        public string VisibleName
        {
            get => this.source.VisibleName;
            set
            {
                this.source.VisibleName = value;
                this.OnPropertyChanged(nameof(this.VisibleName));
            }
        }

        public string DescriptionText => this.source.Type switch
        {
            TagType.Number => "Укажите значения как [мин];[по умолчанию];[макс] или [мин];[макс]",
            TagType.LocalConstant => "Значение константы",
            TagType.HiddenField => "Значение, которое будет использовано, если скрипт вернет пустую строку",
            TagType.LocalEnumeration => "Предлагаемые значения (для разделения используйте символ \";\")",
            TagType.Relation => "Укажите таблицу и поле",
            TagType.Date => "Укажите правило форматирования",
            TagType.Table => "Укажите названия столбцов (для разделения используйте символ \";\")",
            _ => "Значение по умолчанию",
        };
        public int OrderNumber
        {
            get => this.source.OrderNumber;
            set
            {
                if (value is >= 0 and <= 50)
                {
                    this.source.OrderNumber = value;
                    this.OnPropertyChanged(nameof(this.OrderNumber));
                }
            }
        }
        public List<string> Values
        {
            get
            {
                switch (this.source.Type)
                {
                    case TagType.GlobalConstant:
                    default:
                        return this.GetConstants();
                    case TagType.GlobalEnumeration:
                        return this.GetEnumerations();
                }
            }
        }
        public string SelectedValue
        {
            get => this.source.Value;
            set
            {
                this.source.Value = value;
                this.OnPropertyChanged(nameof(this.SelectedValue));
            }
        }
        public Visibility ComboValuesVisibility
        {
            get
            {
                switch (this.source.Type)
                {
                    case TagType.GlobalConstant:
                    case TagType.GlobalEnumeration:
                        return Visibility.Visible;
                    default:
                        return Visibility.Collapsed;
                }
            }
        }
        public List<string> GetConstants()
        {
            using Parameter p = new();
            return p.GetConstantsList();
        }
        public List<string> GetEnumerations()
        {
            using Parameter p = new();
            return p.GetEnumeratorsList();
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
            get => this.source.Name;
            set
            {
                if (value != this.source.Name)
                {
                    this.source.Name = value;
                    this.OnPropertyChanged(nameof(this.NameOfTag));
                }
            }
        }
        public string DefaultValue
        {
            get => this.source.Value;
            set
            {
                if (value != this.source.Value)
                {
                    this.source.Value = value;
                    this.OnPropertyChanged(nameof(this.DefaultValue));
                }
            }
        }
        public Visibility DescriptionVisibility
        {
            get
            {
                switch (this.source.Type)
                {
                    default:
                        return Visibility.Visible;
                    case TagType.LocalConstant:
                    case TagType.GlobalConstant:
                    case TagType.HiddenField:
                    case TagType.Generator:
                    case TagType.Macrogenerator:
                        return Visibility.Collapsed;
                }
            }
        }

        public string Description
        {
            get => this.source.Description;
            set
            {
                if (value != this.source.Description)
                {
                    this.source.Description = value;
                    this.OnPropertyChanged(nameof(this.Description));
                }
            }
        }

        public string TypeOfTagValue
        {
            get => this.SerializeToInput(this.source.Type);
            set
            {
                this.source.Type = this.SerializeFromInput(value);
                this.OnPropertyChanged(nameof(this.TypeOfTagValue));
                this.OnPropertyChanged(nameof(this.DescriptionText));
                this.OnPropertyChanged(nameof(this.ButtonRelationVisibility));
                this.OnPropertyChanged(nameof(this.ButtonGeneratorVisibility));
                this.OnPropertyChanged(nameof(this.DefaultValueVisibility));
                this.OnPropertyChanged(nameof(this.DescriptionVisibility));
                this.OnPropertyChanged(nameof(this.ComboValuesVisibility));
                this.OnPropertyChanged(nameof(this.Values));
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
                    this.Description = "";
                    return TagType.LocalConstant;
                case "8":
                    this.Description = "";
                    return TagType.GlobalConstant;
                case "9":
                    this.Description = "";
                    return TagType.HiddenField;
                case "10":
                    this.Description = "";
                    this.DefaultValue = "";
                    return TagType.Generator;
                case "11":
                    this.Description = "";
                    this.DefaultValue = "";
                    return TagType.Macrogenerator;
                case "12":
                    this.Description = "";
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
        public Visibility ButtonRelationVisibility => this.source.Type == TagType.Relation ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ButtonTableVisibility => this.source.Type == TagType.Table ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ButtonGeneratorVisibility => this.source.Type is TagType.Generator or TagType.Macrogenerator ? Visibility.Visible : Visibility.Collapsed;
        public Visibility DefaultValueVisibility
        {
            get
            {
                switch (this.source.Type)
                {
                    default:
                        return Visibility.Visible;
                    case TagType.Relation:
                    case TagType.GlobalConstant:
                    case TagType.GlobalEnumeration:
                    case TagType.Generator:
                    case TagType.Macrogenerator:
                        return Visibility.Collapsed;
                }
            }
        }
    }
}
#endregion