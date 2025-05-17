using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using Incas.Objects.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для FieldsGroupListVisibilitySettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class FieldsGroupListVisibilitySettings : StaticAutoUIBase
    {
        protected override string FinishButtonText { get => "Сохранить настройку"; }
        #region Data
        [Description("Видимые поля в списках")]
        public CheckedList ListOfFields { get; set; }
        #endregion

        public FieldsGroupListVisibilitySettings(ObservableCollection<FieldViewModel> fields)
        {
            Dictionary<CheckedItem, bool> list = new();
            foreach (FieldViewModel field in fields)
            {
                list.Add(new() { Target = field, Name = field.Name }, field.ListVisibility);
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
                fvm.ListVisibility = pair.Value;
            }
        }
        #endregion
    }
}
