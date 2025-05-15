using ICSharpCode.AvalonEdit;
using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Incas.Scripting.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для ScriptHelperReplace.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class ScriptHelperReplace : StaticAutoUIBase
    {
        protected override string FinishButtonText { get => "Заменить"; }
        #region Data
        private TextEditor editor { get; set; }

        [Description("Старое значение")]
        [StringLength(1200)]
        public string OldValue { get; set; }

        [Description("Новое значение")]
        [StringLength(1200)]
        public string NewValue { get; set; }
        #endregion

        public ScriptHelperReplace(TextEditor editor)
        {
            this.editor = editor;
            this.OldValue = editor.SelectedText;
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
            this.editor.Text = this.editor.Text.Replace(this.OldValue, this.NewValue);
        }
        #endregion
    }
}
