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
using System.Windows.Media;

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
        private static Dictionary<FieldType, FieldTypeDescription> fieldTypes = new()
        {
            { FieldType.Variable, new() { Name="Короткий текст", Description="Ручной ввод", ColorBrush= new SolidColorBrush(new System.Windows.Media.Color() { R=52, G=201, B=36 }) } },
            { FieldType.Text, new() { Name="Многострочный текст", Description="Ручной ввод", ColorBrush= new SolidColorBrush(new System.Windows.Media.Color() { R=52, G=201, B=36 }) } },

            { FieldType.LocalEnumeration, new() { Name="Перечисление", Description="Выпадающий список", ColorBrush = new SolidColorBrush(new System.Windows.Media.Color() { R=245, G=166, B=35 }) } },
            { FieldType.GlobalEnumeration, new() { Name="Глобальное перечисление", Description="Выпадающий список", ColorBrush = new SolidColorBrush(new System.Windows.Media.Color() { R=245, G=166, B=35 })} },
            { FieldType.Date, new() { Name="Дата", Description="Выбор даты", ColorBrush= new SolidColorBrush(new System.Windows.Media.Color() { R=245, G=166, B=35 })} },
            { FieldType.Number, new() { Name="Целочисленное число", Description="Ввод числа", ColorBrush = new SolidColorBrush(new System.Windows.Media.Color() { R=245, G=166, B=35 })} },
            { FieldType.Boolean, new() { Name="Логический флаг", Description="Флажок (Да/Нет)", ColorBrush = new SolidColorBrush(new System.Windows.Media.Color() { R=245, G=166, B=35 })} },

            { FieldType.LocalConstant, new() { Name="Константа", Description="Неизменяемое значение", ColorBrush = new SolidColorBrush(new System.Windows.Media.Color() { R=255, G=0, B=51 }) } },
            { FieldType.GlobalConstant, new() { Name="Глобальная константа", Description="Неизменяемое значение", ColorBrush= new SolidColorBrush(new System.Windows.Media.Color() { R=255, G=0, B=51 })} },
            { FieldType.HiddenField, new() { Name="Скрытое поле", Description="Скриптовое поле", ColorBrush= new SolidColorBrush(new System.Windows.Media.Color() { R=255, G=0, B=51 })} },

            { FieldType.Relation, new() { Name="Объект", Description="Выбор объекта", ColorBrush= new SolidColorBrush(new System.Windows.Media.Color() { R=139, G=0, B=255 }) } },
            { FieldType.Table, new() { Name="Таблица", Description="Заполнение таблицы", ColorBrush= new SolidColorBrush(new System.Windows.Media.Color() { R=139, G=0, B=255 })} },
        };
        public Dictionary<FieldType, FieldTypeDescription> FieldTypes
        {
            get
            {
                return fieldTypes;
            }
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

        public KeyValuePair<FieldType, FieldTypeDescription> TypeOfFieldValue
        {
            get
            {
                return new(this.Source.Type, this.FieldTypes[this.Source.Type]);
            }
            set
            {
                this.Source.Type = value.Key;
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
        #region Not Standart Properties

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