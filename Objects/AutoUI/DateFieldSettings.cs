﻿using Incas.DialogSimpleForm.Components;
using Incas.Objects.Components;
using IncasEngine.ObjectiveEngine.FieldComponents;
using IncasEngine.ObjectiveEngine.Models;
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
        public Selector Format { get; set; }

        [Description("Минимальная дата")]
        public DateTime StartDate { get; set; }

        [Description("Максимальная дата")]
        public DateTime EndDate { get; set; }
        #endregion

        public DateFieldSettings(Field field)
        {
            this.Source = field;
            this.GetBaseData();
            this.Format = new([]);
            this.Format.Pairs.Add(DateFormats.Usual, "01.01.2001");
            this.Format.Pairs.Add(DateFormats.Full, "01 января 2001");
            this.Format.Pairs.Add(DateFormats.FullWithQuotes, "«01» января 2001");
            try
            {
                DateFieldData df = field.GetDateFieldData();
                this.StartDate = df.StartDate == DateTime.MinValue? DateTime.Today : df.StartDate;
                this.EndDate = df.EndDate == DateTime.MinValue ? DateTime.Today.AddYears(10) : df.EndDate;
                this.Format.SetSelection(df.Format);
            }
            catch
            {
                this.StartDate = DateTime.Today.AddYears(-1);
                this.EndDate = DateTime.Today.AddYears(10);
            }
        }

        #region Functionality
        public override void Save()
        {
            this.SaveBaseData();
            DateFieldData df = new()
            {
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                Format = (DateFormats)this.Format.SelectedObject
            };
            this.Source.SetDateFieldData(df);
        }
        #endregion
    }
}
