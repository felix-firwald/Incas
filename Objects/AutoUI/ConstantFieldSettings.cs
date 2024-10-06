using Incas.DialogSimpleForm.Components;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для ConstantFieldSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class ConstantFieldSettings : AutoUIBase
    {
        protected override string FinishButtonText { get => "Применить настройки"; }
        #region Data
        private Incas.Objects.Models.Field Source;

        [Description("Значение")]
        [StringLength(1200)]
        public string Text { get; set; }
        #endregion

        public ConstantFieldSettings(Incas.Objects.Models.Field field)
        {
            this.Source = field;
            this.Text = field.Value;
        }

        #region Functionality
        public override void Save()
        {
            this.Source.Value = this.Text;
        }
        #endregion
    }
}
