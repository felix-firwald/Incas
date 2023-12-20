using DocumentFormat.OpenXml.Office2010.CustomUI;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.ViewModels
{
    class VM_Tag : VM_Base
    {

        private Tag mainTag;
        public Dictionary<int, string> enumerations;

        public VM_Tag(Tag tag, Dictionary<int, string> enums)
        {
            this.mainTag = tag;
            this.enumerations = enums;
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
        public string TypeOfTagValue
        {
            get { return SerializeToInput(mainTag.type); }
            set
            {
                 mainTag.type = SerializeFromInput(value);
                 OnPropertyChanged(nameof(TypeOfTagValue));
            }
        }

        public string EnumerationValue
        {
            get { return this.enumerations[mainTag.enumeration]; }
            set
            {
                mainTag.enumeration = int.Parse(value);
                OnPropertyChanged(nameof(EnumerationValue));
            }
        }


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
                    return TypeOfTag.LocalConstant;
                case "3":
                    return TypeOfTag.LocalEnumeration;
                case "4":
                    return TypeOfTag.GlobalEnumeration;
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
                case TypeOfTag.LocalConstant:
                    return "2";
                case TypeOfTag.LocalEnumeration:
                    return "3";
                case TypeOfTag.GlobalEnumeration:
                    return "4";
            }
        }
        #endregion
    }
}
