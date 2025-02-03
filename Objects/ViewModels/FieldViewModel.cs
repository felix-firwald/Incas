using Incas.Core.ViewModels;
using Incas.Objects.Components;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Exceptions;
using IncasEngine.ObjectiveEngine.FieldComponents;
using IncasEngine.ObjectiveEngine.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
        public string SelectedValue
        {
            get => this.Source.Value;
            set
            {
                this.Source.Value = value;
                this.OnPropertyChanged(nameof(this.SelectedValue));
            }
        }

        public string NameOfField
        {
            get => this.Source.Name;
            set
            {
                if (value != this.Source.Name)
                {
                    this.Source.Name = value
                        .Replace(" ", "_")
                        .Replace(".", "_")
                        .Replace("$", "_");
                    this.OnPropertyChanged(nameof(this.NameOfField));
                }
            }
        }

        public string TypeOfFieldValue
        {
            get => this.SerializeToInput(this.Source.Type);
            set
            {
                this.Source.Type = this.SerializeFromInput(value);
                this.OnPropertyChanged(nameof(this.TypeOfFieldValue));
                this.OnPropertyChanged(nameof(this.ActionBindingEnabled));
                this.OnPropertyChanged(nameof(this.EventBindingEnabled));
            }
        }
        public bool ActionBindingEnabled
        {
            get
            {
                switch (this.Source.Type)
                {
                    case FieldType.LocalConstant:
                    case FieldType.GlobalConstant:
                    case FieldType.HiddenField:
                        return false;
                    default:
                        return true;
                }
            }
        }
        public bool EventBindingEnabled
        {
            get
            {
                switch (this.Source.Type)
                {
                    case FieldType.LocalConstant:
                    case FieldType.GlobalConstant:
                        return false;
                    default:
                        return true;
                }
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
                    return FieldType.Text;
                case "2":
                    return FieldType.LocalEnumeration;
                case "3":
                    return FieldType.GlobalEnumeration;
                case "4":
                    return FieldType.Date;
                case "5":
                    return FieldType.Number;
                case "6":
                    return FieldType.LocalConstant;
                case "7":
                    return FieldType.GlobalConstant;
                case "8":
                    return FieldType.HiddenField;
                case "9":
                    return FieldType.Relation;
                case "10":
                    return FieldType.Generator;
                case "11":
                    return FieldType.Table;
            }
        }
        public string SerializeToInput(FieldType tot)
        {
            return tot switch
            {
                FieldType.Text => "1",
                FieldType.LocalEnumeration => "2",
                FieldType.GlobalEnumeration => "3",
                FieldType.Date => "4",
                FieldType.Number => "5",
                FieldType.LocalConstant => "6",
                FieldType.GlobalConstant => "7",
                FieldType.HiddenField => "8",
                FieldType.Relation => "9",
                FieldType.Generator => "10",
                FieldType.Table => "11",
                _ => "0",
            };
        }

        public void CheckField()
        {
            if (string.IsNullOrWhiteSpace(this.NameOfField))
            {
                throw new FieldDataFailed($"Одному из полей не присвоено имя. Настройте поле, а затем попробуйте снова.");
            }
            try
            {
                switch (this.Source.Type)
                {
                    case FieldType.Text:
                    case FieldType.Variable:
                        break;
                    case FieldType.LocalEnumeration:
                        JsonConvert.DeserializeObject<List<string>>(this.Source.Value);
                        break;
                    case FieldType.GlobalEnumeration:
                        Guid.Parse(this.Source.Value);
                        break;
                    case FieldType.Relation:
                        BindingData bd = JsonConvert.DeserializeObject<BindingData>(this.Source.Value);
                        if (bd.Class == Guid.Empty || bd.Field == Guid.Empty)
                        {
                            throw new FieldDataFailed($"Не определена привязка у поля [{this.Source.Name}] (\"{this.Source.VisibleName}\"). Настройте поле, а затем попробуйте снова.");
                        }
                        break;
                    case FieldType.GlobalConstant:
                        Guid.Parse(this.Source.Value);
                        break;
                    case FieldType.HiddenField:
                        break;
                    case FieldType.Number:
                        JsonConvert.DeserializeObject<NumberFieldData>(this.Source.Value);
                        break;
                    case FieldType.Date:
                        JsonConvert.DeserializeObject<DateFieldData>(this.Source.Value);
                        break;
                    case FieldType.Table:
                        JsonConvert.DeserializeObject<TableFieldData>(this.Source.Value);
                        break;
                }
            }
            catch
            {
                throw new FieldDataFailed($"Поле [{this.Source.Name}] (\"{this.Source.VisibleName}\") не настроено. Настройте поле, а затем попробуйте снова.");
            }
        }
    }
}
#endregion