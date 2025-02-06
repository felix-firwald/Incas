using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Incas.Rendering.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для ParseSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class ParseSettings : AutoUIBase
    {
        #region Data
        [Description("Паттерн")]
        [StringLength(1200)]
        public string Pattern { get; set; }

        [Description("Источник парсинга")]
        [StringLength(1200)]
        public string Source { get; set; }
        #endregion

        public ParseSettings()
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
