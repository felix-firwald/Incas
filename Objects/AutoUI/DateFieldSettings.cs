using Incas.Core.AutoUI;
using Incas.Core.Classes;
using System;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для DateFieldSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class DateFieldSettings : AutoUIBase
    {
        #region Data
        private Incas.Objects.Models.Field Source;

        [Description("Выбор форматирования даты из списка")]
        public ComboSelector Format { get; set; }

        [Description("Минимальная дата")]
        public DateTime StartDate { get; set; }

        [Description("Максимальная дата")]
        public DateTime EndDate { get; set; }
        #endregion

        public DateFieldSettings(Incas.Objects.Models.Field field)
        {
            this.Source = field;
            this.Format.Pairs = new()
            {
                {
                    "dd.MM.yyyy", "01.01.2001"
                },
                {
                    "dd/MM/yyyy", "01/01/2001"
                },
                {
                    "dd MMMM yyyy", "01 января 2001"
                },
            };
            this.Format.SetSelection(field.Value);
        }

        #region Functionality
        public void Save()
        {

        }
        #endregion
    }
}
