using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using IncasEngine.Workspace;
using System.ComponentModel;

namespace Incas.Admin.AutoUI
{
    public class WorkspaceSettings : AutoUIBase
    {
        private WorkspaceDefinition data;
        [Description("Имя рабочего пространства")]
        public string Name
        {
            get => this.data.Name;
            set => this.data.Name = value;
        }

        [Description("Режим работы")]
        public Selector WorkspaceMode { get; set; }

        [Description("Требовать пароль при существенных изменениях")]
        public bool RequirePassword
        {
            get => this.data.SignificantChangesRequirePassword;
            set => this.data.SignificantChangesRequirePassword = value;
        }

        public override void Load()
        {
            this.data = ProgramState.CurrentWorkspace.GetDefinition(true);
            this.WorkspaceMode = new(new()
            {
                { WorkspaceDefinition.WorkspaceMode.DesktopUsual, "Десктоп-клиент (SQLite)" },
                { WorkspaceDefinition.WorkspaceMode.DesktopWithPostgres, "Десктоп-клиент (PostgreSQL)" },
                { WorkspaceDefinition.WorkspaceMode.Server, "Клиент-сервер (IncasServer + SQLite)" },
                { WorkspaceDefinition.WorkspaceMode.ServerWithPostgres, "Клиент-сервер (IncasServer + PostgreSQL)" }
            });
            this.WorkspaceMode.SetSelection(this.data.Mode);
        }
        public override void Save()
        {
            this.data.Mode = (WorkspaceDefinition.WorkspaceMode)this.WorkspaceMode.SelectedObject;
            ProgramState.CurrentWorkspace.UpdateDefinition(this.data);
        }
    }
}
