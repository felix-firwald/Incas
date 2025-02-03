using Incas.DialogSimpleForm.Components;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Models;
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

        public DataSearch(ClassData data)
        {
            this.ComboSelector = new([]);
            //this.ComboSelector.Pairs.Add("Наименование", "Наименование");
            //if (data.ClassType == Components.ClassType.Document)
            //{
            //    this.ComboSelector.Pairs.Add("Дата создания", "Дата создания");
            //}
            foreach (Field f in data.Fields)
            {
                this.ComboSelector.Pairs.Add(f, f.VisibleName);
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
        #endregion
    }
}
