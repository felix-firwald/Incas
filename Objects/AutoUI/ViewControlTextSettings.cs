using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.Core.Extensions;
using Incas.DialogSimpleForm.Components;
using IncasEngine.Core;
using IncasEngine.ObjectiveEngine.Common.FunctionalityUtils.CustomForms;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static IncasEngine.ObjectiveEngine.Common.FunctionalityUtils.CustomForms.ViewControl.ViewControlTextSettings;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для ViewControlTextSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class ViewControlTextSettings : StaticAutoUIBase
    {
        #region Data
        private ViewControl.ViewControlTextSettings result;
        [Description("Текст")]
        [CanBeNull]
        [StringLength(500)]
        public string Text { get; set; }

        [Description("Цвет текста")]
        public System.Windows.Media.Color Color { get; set; }

        [Description("Размер текста")]
        public Selector SizeSelector { get; set; }

        [Description("Жирный")]
        public bool IsBold { get; set; }

        [Description("Курсив")]
        public bool IsItalic { get; set; }
        #endregion

        public ViewControlTextSettings(ViewControl.ViewControlTextSettings settings)
        {
            if (settings == null)
            {
                this.result = new();
            }
            else
            {
                this.result = settings;
            }           
            this.Text = this.result.Text;
            this.Color = this.result.Color.AsMediaColor();
            this.SizeSelector = new(
                new()
                {
                    { TextSettingsSize.Small, "Маленький" },
                    { TextSettingsSize.Medium, "Средний" },
                    { TextSettingsSize.Large, "Большой" }
                }
            );
            this.SizeSelector.SetSelection(this.result.Size);
            this.IsBold = this.result.Bold;
            this.IsItalic = this.result.Italic;
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
            this.result.Text = this.Text;
            this.result.Color = this.Color.AsIncasEngineColor();
            this.result.Size = (TextSettingsSize)this.SizeSelector.SelectedObject;
            this.result.Bold = this.IsBold;
            this.result.Italic = this.IsItalic;
        }
        public ViewControl.ViewControlTextSettings GetResult()
        {
            return this.result;
        }
        #endregion
    }
}
