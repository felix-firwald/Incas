using Incas.DialogSimpleForm.Components;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
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
        public Selector Selector { get; set; }
        #endregion

        public TemplateSelection(ClassData data)
        {
            this.Selector = new([]);
            foreach (TemplateData item in data.Templates.Values)
            {
                this.Selector.Pairs.Add(item, item.Name);
            }
            this.Selector.SetSelectionByIndex(0);
        }
        public TemplateData GetSelectedPath()
        {
            return (TemplateData)this.Selector.SelectedObject;
        }

        #region Functionality

        #endregion
    }
}
