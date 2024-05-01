using Models;
using System.Windows;

namespace Incubator_2.ViewModels
{
    internal class VM_Tag : VM_Base
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
                switch (this.mainTag.type)
                {
                    case TypeOfTag.Variable:
                    case TypeOfTag.Text:
                    default:
                        return "Значение по умолчанию";
                    case TypeOfTag.Number:
                        return "Укажите значения как [мин];[по умолчанию];[макс] или [мин];[макс]";
                    case TypeOfTag.LocalConstant:
                        return "Значение константы";
                    case TypeOfTag.HiddenField:
                        return "Значение, которое будет использовано, если скрипт вернет пустую строку";
                    case TypeOfTag.LocalEnumeration:
                        return "Предлагаемые значения (для разделения используйте символ \";\")";
                    case TypeOfTag.Relation:
                        return "Укажите таблицу и поле";
                    case TypeOfTag.Date:
                        return "Укажите правило форматирования";
                    case TypeOfTag.Table:
                        return "Укажите названия столбцов (для разделения используйте символ \";\")";
                }
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
                    case TypeOfTag.LocalConstant:
                    case TypeOfTag.HiddenField:
                    case TypeOfTag.Generator:
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
        public TypeOfTag SerializeFromInput(string val)
        {
            switch (val)
            {
                case "0":
                default:
                    return TypeOfTag.Variable;
                case "1":
                    return TypeOfTag.Text;
                case "2":
                    return TypeOfTag.LocalEnumeration;
                case "3":
                    return TypeOfTag.Relation;
                case "4":
                    return TypeOfTag.Date;
                case "5":
                    return TypeOfTag.Number;
                case "6":
                    this.Description = "";
                    return TypeOfTag.LocalConstant;
                case "7":
                    this.Description = "";
                    return TypeOfTag.HiddenField;
                case "8":
                    this.Description = "";
                    this.DefaultValue = "";
                    return TypeOfTag.Generator;
                case "9":
                    this.Description = "";
                    return TypeOfTag.Table;
            }
        }
        public string SerializeToInput(TypeOfTag tot)
        {
            return tot switch
            {
                TypeOfTag.Text => "1",
                TypeOfTag.LocalEnumeration => "2",
                TypeOfTag.Relation => "3",
                TypeOfTag.Date => "4",
                TypeOfTag.Number => "5",
                TypeOfTag.LocalConstant => "6",
                TypeOfTag.HiddenField => "7",
                TypeOfTag.Generator => "8",
                TypeOfTag.Table => "9",
                _ => "0",
            };
        }
        public Visibility ButtonRelationVisibility
        {
            get
            {
                if (this.mainTag.type == TypeOfTag.Relation)
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
                if (this.mainTag.type == TypeOfTag.Generator)
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
                    case TypeOfTag.Relation:
                    case TypeOfTag.Generator:
                        return Visibility.Collapsed;
                }
            }
        }

        #endregion
    }
}
