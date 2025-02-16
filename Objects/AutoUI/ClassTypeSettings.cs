using Incas.Core.Attributes;
using Incas.DialogSimpleForm.Components;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Models;
using System;
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
        private Class parentClass;

        [Description("Категория класса")]
        public string Category { get; set; }

        [Description("Имя класса")]
        public string Name { get; set; }

        [Description("Тип класса")]
        public Selector Selector { get; set; }

        [Description("Родитель")]
        [CanBeNull]
        public string Parent
        {
            get
            {
                if (this.parentClass is null)
                {
                    return "";
                }
                return this.parentClass.Name;
            }
            private set
            {

            }
        }
        #endregion

        public ClassTypeSettings()
        {
            this.Initialize();
        }
        public ClassTypeSettings(Class parent)
        {
            Dictionary<object, string> pairs = new()
            {
                { parent.Type, "Унаследован" }
            };
            this.Selector = new(pairs);
            this.Selector.SetSelection(parent.Type);
            this.parentClass = parent;
        }
        private void Initialize()
        {
            Dictionary<object, string> pairs = new()
            {
                { ClassType.Model, "Модель данных" },
                //{ ClassType.StaticModel, "Статическая модель данных" },
                { ClassType.Document, "Модель документа" },
                //{ ClassType.Process, "Модель процесса" },
            };
            this.Selector = new(pairs);
        }

        #region Functionality
        public List<Guid> GetParents()
        {
            List<Guid> result = new();
            if (this.parentClass is not null)
            {
                result = this.parentClass.Parents is not null ? this.parentClass.Parents : new();

                result.Add(this.parentClass.Id);
            }           
            return result;
        }
        public override void Validate()
        {
            using Class cl = new();
            foreach (string c in cl.GetAllClassesNames())
            {
                if (c == this.Name)
                {
                    throw new DialogSimpleForm.Exceptions.SimpleFormFailed("Класс с таким именем уже существует.");
                }
            }
        }
        #endregion
    }
}
