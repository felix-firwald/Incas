using Incas.Core.Attributes;
using Incas.DialogSimpleForm.Components;
using IncasEngine.ObjectiveEngine.Models;
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
        protected Field Source;

        [MaxLength(200)]
        [CanBeNull]
        [Description("Описание поля (для форм)")]
        public string Description { get; set; }

        [Description("Обязательно для заполнения")]
        public bool NotNull { get; set; }

        [Description("Только чтение")]
        public bool ReadOnly { get; set; }

        #endregion

        #region Functionality
        protected void GetBaseData()
        {
            this.NotNull = this.Source.NotNull;
            this.ReadOnly = this.Source.ReadOnly;
            this.Description = this.Source.Description;
        }
        protected void SaveBaseData()
        {
            this.Source.NotNull = this.NotNull;
            this.Source.ReadOnly = this.ReadOnly;
            this.Source.Description = this.Description;
        }
        #endregion
    }
}
