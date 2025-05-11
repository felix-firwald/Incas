using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using Incas.DialogSimpleForm.Exceptions;
using IncasEngine.ObjectiveEngine.Common.FunctionalityUtils.CustomForms;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для BaseControlSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class ViewControlTableSettings : StaticAutoUIBase
    {
        #region Data
        private ViewControl.ViewControlTableSettings source { get; set; }

        [Description("Минимальная высота")]
        public int MinHeight { get; set; }
        [Description("Начальная высота")]
        public int Height { get; set; }
        [Description("Максимальная высота")]
        public int MaxHeight { get; set; }
        #endregion

        public ViewControlTableSettings(ViewControl.ViewControlTableSettings settings)
        {
            this.source = settings;
            if (this.source is null)
            {
                this.source = new();
            }
            else
            {
                this.MinHeight = this.source.MinHeight;
                this.Height = this.source.Height;
                this.MaxHeight = this.source.MaxHeight;
            }
        }

        #region Functionality
        public override void Load()
        {
            
        }

        public override void Validate()
        {
            if (this.MaxHeight < this.MinHeight || this.MaxHeight < this.Height)
            {
                throw new SimpleFormFailed("Недопустимое значение максимальной высоты (меньше чем другие величины).");
            }
        }

        public override void Save()
        {
            
        }
        public ViewControl.ViewControlTableSettings GetResult()
        {
            return this.source;
        }
        #endregion
    }
}
