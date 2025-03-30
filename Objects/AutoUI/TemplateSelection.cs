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
        protected override string FinishButtonText { get => "Начать рендеринг"; }
        #region Data
        [Description("Шаблон для рендеринга")]
        public Selector Selector { get; set; }
        #endregion

        public TemplateSelection(DocumentClassData data)
        {
            this.Selector = new([]);
            foreach (Template item in data.Documents)
            {
                this.Selector.Pairs.Add(item, item.Name);
            }
            this.Selector.SetSelectionByIndex(0);
        }
        public Template GetSelectedPath()
        {
            return (Template)this.Selector.SelectedObject;
        }

        #region Functionality

        #endregion
    }
}
