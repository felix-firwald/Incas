using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.Collections.Generic;

namespace Incas.Objects.Processes.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для DocumentSelectorAutoUI.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class DocumentSelectorAutoUI : AutoUIBase
    {
        protected override string FinishButtonText { get => "Добавить документ"; }
        #region Data
        public Selector ClassSelector { get; set; }

        #endregion

        public DocumentSelectorAutoUI()
        {
            using (Class cl = new())
            {
                List<ClassItem> items = cl.GetClassItemsByType(IncasEngine.ObjectiveEngine.Classes.ClassType.Document);
                Dictionary<object, string> dict = new();
                foreach (ClassItem item in items)
                {
                    dict.Add(item, item.Name);
                }
                this.ClassSelector = new(dict);
            }
        }

        #region Functionality
        public override void Load()
        {

        }

        public override void Validate()
        {

        }

        public override void Save()
        {

        }
        public ClassItem GetSelectedDocument()
        {
            return (ClassItem)this.ClassSelector.SelectedObject;
        }
        #endregion
    }
}
