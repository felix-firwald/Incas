using Common;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Incubator_2.ViewModels
{
    class VM_Tag : VM_Base
    {
        private Tag mainTag;
        // сделать проверку по id, если id есть, то update, если нет - add
        public VM_Tag(Tag tag)
        {
            this.mainTag = tag;
        }

        public string DescriptionText
        {
            get
            {
                switch(mainTag.type)
                {
                    case TypeOfTag.Variable:
                    case TypeOfTag.Text:
                    default:
                        return "Значение по умолчанию";
                    case TypeOfTag.LocalConstant:
                        return "Значение константы";
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

        public string NameOfTag
        {
            get { return mainTag.name; }
            set
            {
                if (value != mainTag.name)
                {
                    mainTag.name = value;
                    OnPropertyChanged(nameof(NameOfTag));
                }
            }
        }
        public string DefaultValue
        {
            get { return mainTag.value; }
            set
            {
                if (value != mainTag.value)
                {
                    mainTag.value = value;
                    OnPropertyChanged(nameof(DefaultValue));
                }
            }
        }
        public Visibility DescriptionVisibility
        {
            get
            {
                switch (mainTag.type)
                {
                    default:
                        return Visibility.Visible;
                    case TypeOfTag.LocalConstant:
                    case TypeOfTag.Generator:
                    case TypeOfTag.Table:
                        return Visibility.Collapsed;
                }
            }
        }

        public string Description
        {
            get
            {
                return mainTag.description;
            }
            set
            {
                if (value != mainTag.description)
                {
                    mainTag.description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public string TypeOfTagValue
        {
            get { return SerializeToInput(mainTag.type); }
            set
            {
                mainTag.type = SerializeFromInput(value);
                OnPropertyChanged(nameof(TypeOfTagValue));
                OnPropertyChanged(nameof(DescriptionText));
                OnPropertyChanged(nameof(ButtonRelationVisibility));
                OnPropertyChanged(nameof(ButtonGeneratorVisibility));
                OnPropertyChanged(nameof(DefaultValueVisibility));
                OnPropertyChanged(nameof(DescriptionVisibility));
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
                    Description = "";
                    return TypeOfTag.LocalConstant;
                case "6":
                    Description = "";
                    DefaultValue = "";
                    return TypeOfTag.Generator;
                case "7":
                    Description = "";
                    return TypeOfTag.Table;
            }
        }
        public string SerializeToInput(TypeOfTag tot)
        {
            switch(tot)
            {
                case TypeOfTag.Variable:
                default:
                    return "0";
                case TypeOfTag.Text:
                    return "1";
                case TypeOfTag.LocalEnumeration:
                    return "2";
                case TypeOfTag.Relation:
                    return "3";
                case TypeOfTag.Date:
                    return "4";
                case TypeOfTag.LocalConstant:
                    return "5";
                case TypeOfTag.Generator:
                    return "6";
                case TypeOfTag.Table:
                    return "7";
            }
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
