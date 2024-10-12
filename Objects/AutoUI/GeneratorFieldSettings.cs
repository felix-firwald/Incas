using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для GeneratorFieldSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class GeneratorFieldSettings : AutoUIBase
    {
        #region Data
        [Description("Выбор генератора")]
        public Selector Selector { get; set; }

        [Description("Обязательно для заполнения")]
        public bool NotNull { get; set; }
        #endregion

        public GeneratorFieldSettings()
        {
            
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
            
        }
        #endregion
    }
}
