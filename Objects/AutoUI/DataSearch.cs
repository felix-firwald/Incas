using DocumentFormat.OpenXml.Bibliography;
using Incas.DialogSimpleForm.Components;
using Incas.Miniservices.UserStatistics;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для DataSearch.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class DataSearch : AutoUIBase
    {
        protected override string FinishButtonText { get => "Найти"; }
        #region Data
        [Description("Поле для поиска")]
        public Selector ComboSelector { get; set; }

        [Description("Значение")]
        public string Value { get; set; }

        [Description("По полному совпадению")]
        public bool OnlyEqual { get; set; }
        #endregion

        public DataSearch(IClass cl, ClassDataBase data)
        {
            this.ComboSelector = new([]);
            Guid id = StatisticsManager.GetDefaultFieldForSearch(cl);
            foreach (Field f in data.GetVisibleListFields())
            {
                this.ComboSelector.Pairs.Add(f, f.VisibleName);
                if (id == f.Id)
                {
                    this.ComboSelector.SetSelection(f);
                }
            }
        }

        #region Functionality
        public FieldData GetData()
        {
            FieldData f = new()
            {
                ClassField = (Field)this.ComboSelector.SelectedObject,
                Value = this.Value
            };
            return f;
        }
        public override void Save()
        {
            
        }
        #endregion
    }
}
