using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using Incas.Scripting.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Incas.Scripting.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для ScriptHelperCreateDialog.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class ScriptHelperCreateDialog : StaticAutoUIBase, IScriptHelper
    {
        protected override string FinishButtonText { get => "Вставить код"; }
        #region Data
        [Description("Название")]
        public string FormName { get; set; }

        [Description("Тип окна")]
        public Selector TargetType { get; set; }

        [Description("Текст диалогового окна")]
        [StringLength(1200)]
        public string FormText { get; set; }
        #endregion

        public ScriptHelperCreateDialog()
        {
            Dictionary<object, string> types = new()
            {
                { "show_info_dialog", "Информирующий диалог" },
                { "show_exclamation_dialog", "Восклицательный диалог" },
                { "show_error_dialog", "Диалог ошибки" },
            };
            this.TargetType = new(types);
            this.TargetType.SetSelection("show_info_dialog");
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

        public string GetScript()
        {
            string result = $"incas.{this.TargetType.SelectedObject}('{this.FormName}', '{this.FormText}')";
            return result;
        }
        #endregion
    }
}
