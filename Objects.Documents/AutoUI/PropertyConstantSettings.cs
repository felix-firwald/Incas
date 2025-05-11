using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Incas.Objects.Documents.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для PropertyConstantSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class PropertyConstantSettings : StaticAutoUIBase
    {
        protected override string FinishButtonText { get => "Сохранить значение"; set => base.FinishButtonText = value; }
        #region Data
        private TemplateProperty prop;

        [Description("Значение свойства")]
        [StringLength(1200)]
        [CanBeNull]
        public string Text { get; set; }
        #endregion

        public PropertyConstantSettings(TemplateProperty prop)
        {
            this.prop = prop;
            this.Text = prop.Value;
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
            this.prop.Value = this.Text;
        }
        #endregion
    }
}
