using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using IncasEngine.ObjectiveEngine.Common.FunctionalityUtils.CustomForms;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для ViewControlFieldSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class ViewControlFieldSettings : StaticAutoUIBase
    {
        #region Data
        private ViewControl source { get; set; }

        [Description("Спрятать текстовую метку")]
        public bool HiddenText { get; set; }
        #endregion

        public ViewControlFieldSettings(ViewControl source)
        {
            this.source = source;
            this.HiddenText = this.source.TextHidden;
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
            this.source.TextHidden = this.HiddenText;
        }
        #endregion
    }
}
