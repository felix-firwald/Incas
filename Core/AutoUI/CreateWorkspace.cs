using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using IncasEngine.Workspace.Interfaces;
using System.ComponentModel;

namespace Incas.Core.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для CreateWorkspace.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    internal class CreateWorkspace : StaticAutoUIBase, IWorkspaceCreationData
    {
        #region Data     
        [Description("Наименование рабочего пространства")]
        public string WorkspaceName { get; set; }

        [Description("Путь к рабочему пространству")]
        [UrlRequired]
        public string Path { get; set; }

        [Description("Пароль для входа")]
        public string Password { get; set; }
        #endregion

        #region Functionality   
        public override void Save()
        {
            ProgramState.InitWorkspace(this);
        }
        #endregion
    }
}
