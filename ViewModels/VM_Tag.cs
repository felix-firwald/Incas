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
using System.Windows.Input;

namespace Incubator_2.ViewModels
{
    class VM_Tag : VM_Base
    {

        private Tag mainTag;
        private Dictionary<string, int> _enumerations;
        public Dictionary<string, int> enumerations { get { return _enumerations; } }
        

        public VM_Tag(Tag tag, Dictionary<string, int> enums)
        {
            this._enumerations = enums;
            this.mainTag = tag;
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
            get 
            {
                return GetEnumerationById(mainTag.enumeration); 
            }
            set
            {
                this.mainTag.enumeration = int.Parse(value);
                OnPropertyChanged(nameof(EnumerationValue));
            }
        }
        // может лучше по selected index?

        private string GetEnumerationById(int id)
        {
            int counter = 0;
            foreach (KeyValuePair<string, int> kv in this._enumerations)
            {
                if (kv.Value == id) 
                {
                    return counter.ToString();
                }
                counter++;
            }
            return "-";
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
