using Incas.DialogSimpleForm.Components;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для ClassSelector.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class ClassSelector : AutoUIBase
    {
        #region Data
        [Description("Выбор класса")]
        public Selector ComboSelector { get; set; }
        #endregion

        public ClassSelector()
        {
            this.ComboSelector = new([]);
            using Class main = new();
            foreach (Class cl in main.GetAllClasses())
            {
                this.ComboSelector.Pairs.Add(cl.Id, cl.Name);
            }
        }

        #region Functionality
        public Class GetSelectedClass()
        {
            Class cl = new((Guid)this.ComboSelector.SelectedObject);
            return cl;
        }
        public IClassData GetSelectedClassData()
        {
            Class cl = new((Guid)this.ComboSelector.SelectedObject);
            return cl.GetClassData();
        }
        #endregion
    }
}
