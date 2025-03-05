using Incas.Core.Classes;
using Incas.Core.Views.Controls;
using Incas.DialogSimpleForm.Components;
using Incas.Objects.Views.Pages;
using IncasEngine.Core;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.Workspace;
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
                    ObjectEditions objViewer;
                    IObject @object;
                    WorkspaceDefinition def = ProgramState.CurrentWorkspace.GetDefinition();
                    if (def.ServiceGroups.Id == or.Class)
                    {
                        @object = Processor.GetObject(def.ServiceGroups, or.Object);
                        objViewer = new(def.ServiceGroups, @object);
                        DialogsManager.ShowPageWithGroupBox(objViewer, "Поиск: " + def.ServiceGroups.Name, "FIND" + this.link, TabType.Usual);
                    }
                    else if (def.ServiceUsers.Id == or.Class)
                    {
                        @object = Processor.GetObject(def.ServiceUsers, or.Object);
                        objViewer = new(def.ServiceUsers, @object);
                        DialogsManager.ShowPageWithGroupBox(objViewer, "Поиск: " + def.ServiceUsers.Name, "FIND" + this.link, TabType.Usual);
                    }
                    else
                    {
                        Class @class = EngineGlobals.GetClass(or.Class);
                        @object = Processor.GetObject(@class, or.Object);
                        objViewer = new(@class, @object);
                        DialogsManager.ShowPageWithGroupBox(objViewer, "Поиск: " + @class.Name, "FIND" + this.link, TabType.Usual);
                    }                  
                }
                else
                {
                    DialogsManager.ShowExclamationDialog("Ссылка на объект или класс не установлена.", "Поиск прерван");
                }
            }
            catch
            {
                DialogsManager.ShowExclamationDialog("Не удалось расшифровать ссылку на объект, либо ссылка отсылает на несуществующий класс.", "Поиск прерван");
            }
        }
        #endregion
    }
}
