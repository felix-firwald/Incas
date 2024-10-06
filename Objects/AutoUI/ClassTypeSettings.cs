using Incas.DialogSimpleForm.Components;
using Incas.Core.Classes;
using Incas.Objects.Components;
using Incas.Objects.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для ClassTypeSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class ClassTypeSettings : AutoUIBase
    {
        protected override string FinishButtonText { get => "Далее"; }
        protected override string CancelButtonText { get => "Отменить создание класса"; }
        #region Data
        [Description("Категория класса")]
        public string Category { get; set; }
        [Description("Имя класса")]
        public string Name { get; set; }
        [Description("Тип класса")]
        public Selector Selector { get; set; }
        #endregion

        public ClassTypeSettings()
        {
            Dictionary<object, string> pairs = new()
            {
                { ClassType.Model, "Модель данных" },
                { ClassType.Document, "Документ (процесс)" }
            };
            this.Selector = new(pairs);
        }

        #region Functionality
        public override void Validate()
        {
            using Class cl = new();
            foreach (Class c in cl.GetAllClasses())
            {
                if (c.name == this.Name)
                {
                    throw new Core.Exceptions.SimpleFormFailed("Класс с таким именем уже существует.");
                }
            }
        }
        #endregion
    }
}
