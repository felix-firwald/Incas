using Incas.Core.Attributes;
using Incas.DialogSimpleForm.Components;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для BaseFieldSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class BaseFieldSettings : AutoUIBase
    {
        protected override string FinishButtonText { get => "Применить настройки"; }
        #region Data
        protected Incas.Objects.Models.Field Source;

        [MaxLength(200)]
        [CanBeNull]
        [Description("Описание поля (для форм)")]
        public string Description { get; set; }

        [Description("Обязательно для заполнения")]
        public bool NotNull { get; set; }

        #endregion

        #region Functionality
        protected void GetBaseData()
        {
            this.NotNull = this.Source.NotNull;
            this.Description = this.Source.Description;
        }
        protected void SaveBaseData()
        {
            this.Source.NotNull = this.NotNull;
            this.Source.Description = this.Description;
        }
        #endregion
    }
}
