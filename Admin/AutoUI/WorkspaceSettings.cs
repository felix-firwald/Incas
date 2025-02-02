using Incas.Core.Classes;
using Incas.Core.Models;
using Incas.DialogSimpleForm.Components;
using Newtonsoft.Json;
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

        [Description("Рабочее пространство заблокировано")]
        public bool WorkspaceLocked
        {
            get => this.data.IsLocked;
            set => this.data.IsLocked = value;
        }
        [Description("Требовать пароль при существенных изменениях")]
        public bool RequirePassword
        {
            get => this.data.SignificantChangesRequirePassword;
            set => this.data.SignificantChangesRequirePassword = value;
        }
        public override void Load()
        {
            this.data = ProgramState.CurrentWorkspace.GetDefinition(true);
        }
        public override void Save()
        {
            ProgramState.CurrentWorkspace.UpdateDefinition(this.data);
        }
    }
}
