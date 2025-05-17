using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using Incas.Objects.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Documents;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для FieldsGroupFilterSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class FieldsGroupFilterSettings : StaticAutoUIBase
    {
        protected override string FinishButtonText { get => "Сохранить настройку"; }
        #region Data
        [Description("Использовать групповую фильтрацию")]
        public bool GroupFilterEnabled { get; set; }

        [Description("Кол-во колонок группировки")]
        public int ColumnsCount { get; set; }

        [Description("Кол-во рядов группировки")]
        public int RowsCount { get; set; }

        [Description("Настройка полей")]
        public CheckedList ListOfFields { get; set; }
        #endregion

        public FieldsGroupFilterSettings(ObservableCollection<FieldViewModel> fields)
        {
            Dictionary<CheckedItem, bool> list = new();
            foreach (FieldViewModel field in fields)
            {
                list.Add(new() { Target = field, Name = field.Name }, field.FilterOn);
            }
            this.ListOfFields = new(list);
        }

        #region Functionality
        public override void Load()
        {

        }

        public override void Validate()
        {

        }

        public override void Save()
        {
            foreach (KeyValuePair<CheckedItem, bool> pair in this.ListOfFields.Pairs)
            {
                FieldViewModel fvm = pair.Key.Target as FieldViewModel;
                fvm.FilterOn = pair.Value;
            }
        }
        #endregion
    }
}
