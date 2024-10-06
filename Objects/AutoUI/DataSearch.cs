using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.Core.AutoUI;
using Incas.Objects.Models;
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
            this.ComboSelector = new(new());
            //this.ComboSelector.Pairs.Add("Наименование", "Наименование");
            //if (data.ClassType == Components.ClassType.Document)
            //{
            //    this.ComboSelector.Pairs.Add("Дата создания", "Дата создания");
            //}
            foreach (Objects.Models.Field f in data.Fields)
            {
                this.ComboSelector.Pairs.Add(f, f.VisibleName);
            }
        }

        #region Functionality
        public Components.FieldData GetData()
        {
            Components.FieldData f = new()
            {
                ClassField = (Models.Field)this.ComboSelector.SelectedObject,
                Value = this.Value
            };
            return f;
        }
        #endregion
    }
}
