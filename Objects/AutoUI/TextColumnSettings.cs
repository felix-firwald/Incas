using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using Incas.Objects.Components;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для TextColumnSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class TextColumnSettings : AutoUIBase
    {
        #region Data
        private TableFieldColumnData Source;

        [CanBeNull]
        [Description("Значение по-умолчанию")]
        public string DefaultText { get; set; }

        [Description("Обязательно для заполнения")]
        public bool NotNull { get; set; }
        #endregion

        public TextColumnSettings(TableFieldColumnData field)
        {
            this.Source = field;
            this.DefaultText = field.Value;
            this.NotNull = field.NotNull;
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
            this.Source.Value = this.DefaultText;
            this.Source.NotNull = this.NotNull;
        }
        #endregion
    }
}
