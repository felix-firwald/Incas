using Incas.Core.Attributes;
using Incas.DialogSimpleForm.Components;
using Incas.Objects.Interfaces;
using Incas.Objects.ViewModels;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.Collections.Generic;
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

        #endregion

        #region Functionality
        protected void GetBaseData()
        {
            this.Description = this.Source.Description;
        }
        protected void SaveBaseData()
        {
            this.Source.Description = this.Description;
        }
        #endregion
    }
}
