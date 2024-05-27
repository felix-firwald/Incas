using Incas.Core.ViewModels;
using IncasEngine.TemplateManager;
using Models;
using System.Windows;

namespace Incubator_2.ViewModels
{
    internal class VM_Tag : BaseViewModel
    {
        private Tag mainTag;
        // сделать проверку по id, если id есть, то update, если нет - add
        public VM_Tag(Tag tag)
        {
            this.mainTag = tag;
        }
        public string VisibleName
        {
            get
            {
                return this.mainTag.visibleName;
            }
            set
            {
                this.mainTag.visibleName = value;
                this.OnPropertyChanged(nameof(this.VisibleName));
            }
        }

        public string DescriptionText
        {
            get
            {
                return this.mainTag.type switch
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
            }
        }
        public int OrderNumber
        {
            get
            {
                return this.mainTag.orderNumber;
            }
            set
            {
                if (value is >= 0 and <= 50)
                {
                    this.mainTag.orderNumber = value;
                    this.OnPropertyChanged(nameof(this.OrderNumber));
                }
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
            get { return this.mainTag.name; }
            set
            {
                if (value != this.mainTag.name)
                {
                    this.mainTag.name = value;
                    this.OnPropertyChanged(nameof(this.NameOfTag));
                }
            }
        }
        public string DefaultValue
        {
            get { return this.mainTag.value; }
            set
            {
                if (value != this.mainTag.value)
                {
                    this.mainTag.value = value;
                    this.OnPropertyChanged(nameof(this.DefaultValue));
                }
            }
        }
        public Visibility DescriptionVisibility
        {
            get
            {
                switch (this.mainTag.type)
                {
                    default:
                        return Visibility.Visible;
                    case TagType.LocalConstant:
                    case TagType.HiddenField:
                    case TagType.Generator:
                        return Visibility.Collapsed;
                }
            }
        }

        public string Description
        {
            get
            {
                return this.mainTag.description;
            }
            set
            {
                if (value != this.mainTag.description)
                {
                    this.mainTag.description = value;
                    this.OnPropertyChanged(nameof(this.Description));
                }
            }
        }

        public string TypeOfTagValue
        {
            get { return this.SerializeToInput(this.mainTag.type); }
            set
            {
                this.mainTag.type = this.SerializeFromInput(value);
                this.OnPropertyChanged(nameof(this.TypeOfTagValue));
                this.OnPropertyChanged(nameof(this.DescriptionText));
                this.OnPropertyChanged(nameof(this.ButtonRelationVisibility));
                this.OnPropertyChanged(nameof(this.ButtonGeneratorVisibility));
                this.OnPropertyChanged(nameof(this.DefaultValueVisibility));
                this.OnPropertyChanged(nameof(this.DescriptionVisibility));
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
                    return TagType.Relation;
                case "4":
                    return TagType.Date;
                case "5":
                    return TagType.Number;
                case "6":
                    this.Description = "";
                    return TagType.LocalConstant;
                case "7":
                    this.Description = "";
                    return TagType.HiddenField;
                case "8":
                    this.Description = "";
                    this.DefaultValue = "";
                    return TagType.Generator;
                case "9":
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
                TagType.Relation => "3",
                TagType.Date => "4",
                TagType.Number => "5",
                TagType.LocalConstant => "6",
                TagType.HiddenField => "7",
                TagType.Generator => "8",
                TagType.Table => "9",
                _ => "0",
            };
        }
        public Visibility ButtonRelationVisibility
        {
            get
            {
                if (this.mainTag.type == TagType.Relation)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        public Visibility ButtonGeneratorVisibility
        {
            get
            {
                if (this.mainTag.type is TagType.Generator)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        public Visibility DefaultValueVisibility
        {
            get
            {
                switch (this.mainTag.type)
                {
                    default:
                        return Visibility.Visible;
                    case TagType.Relation:
                    case TagType.Generator:
                        return Visibility.Collapsed;
                }
            }
        }

        #endregion
    }
}
