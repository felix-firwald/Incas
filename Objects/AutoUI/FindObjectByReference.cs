using Incas.Core.Classes;
using Incas.Core.Views.Controls;
using Incas.DialogSimpleForm.Components;
using Incas.Objects.Views.Pages;
using IncasEngine.Core;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для FindObjectByReference.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class FindObjectByReference : AutoUIBase
    {
        protected override string FinishButtonText { get => "Найти объект"; }
        #region Data

        [Description("Ссылка на объект")]
        public string link { get; set; }
        #endregion

        public FindObjectByReference()
        {

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
            try
            {
                ObjectReference or = ObjectReference.Parse(this.link);
                if (or.IsCorrect)
                {
                    Class @class = EngineGlobals.GetClass(or.Class);
                    IObject @object = Processor.GetObject(@class, or.Object);
                    ObjectEditions objViewer = new(@class, @object);
                    DialogsManager.ShowPageWithGroupBox(objViewer, "Поиск: " + @class.Name, "FIND" + this.link, TabType.Usual);
                }
                else
                {
                    DialogsManager.ShowExclamationDialog("Ссылка на объект или класс не установлена.", "Поиск прерван");
                }
            }
            catch
            {
                DialogsManager.ShowErrorDialog("Не удалось расшифровать ссылку на объект, либо ссылка отсылает на несуществующий класс.");
            }
        }
        #endregion
    }
}
