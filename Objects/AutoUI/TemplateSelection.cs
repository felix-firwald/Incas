using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.Core.AutoUI;
using Incas.Objects.Models;
using Incas.Objects.Components;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для TemplateSelection.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class TemplateSelection : AutoUIBase
    {
        #region Data
        [Description("Шаблон для рендеринга")]
        public ComboSelector Selector { get; set; }
        #endregion

        public TemplateSelection(ClassData data)
        {
            this.Selector = new(new());
            foreach (TemplateData item in data.Templates.Values)
            {
                this.Selector.Pairs.Add(item.File, item.Name);
            }
            this.Selector.SetSelectionByIndex(0);
        }
        public string GetSelectedPath()
        {
            return this.Selector.SelectedObject.ToString();
        }

        #region Functionality

        #endregion
    }
}
