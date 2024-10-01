using Incas.Core.AutoUI;
using Incas.Core.Classes;
using Incas.Objects.Components;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для DateFieldSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class DateFieldSettings : BaseFieldSettings
    {
        #region Data

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
            this.GetBaseData();
            this.Format = new(new());
            this.Format.Pairs.Add(DateFormats.Usual, "01.01.2001");
            this.Format.Pairs.Add(DateFormats.Full, "01 января 2001");
            this.Format.Pairs.Add(DateFormats.FullWithQuotes, "«01» января 2001");
            try
            {
                DateFieldData df = JsonConvert.DeserializeObject<DateFieldData>(field.Value);
                this.StartDate = df.StartDate;
                this.EndDate = df.EndDate;
                this.Format.SetSelection(df.Format);
            }
            catch
            {
                this.StartDate = DateTime.MinValue;
                this.EndDate= DateTime.MaxValue;
            }         
        }

        #region Functionality
        public void Save()
        {
            this.SaveBaseData();
            DateFieldData df = new()
            {
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                Format = (DateFormats)this.Format.SelectedObject
            };
            this.Source.Value = JsonConvert.SerializeObject(df);
        }
        #endregion
    }
}
