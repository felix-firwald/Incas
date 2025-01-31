using Incas.Core.Classes;
using Incas.Core.Models;
using Incas.DialogSimpleForm.Components;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Incas.Admin.AutoUI
{
    public class WorkspaceSettings : AutoUIBase
    {
        private WorkspacePrimarySettings data;
        private Parameter param; 
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
        public override void Load()
        {
            this.param = ProgramState.GetParameter(ParameterType.WORKSPACE, Workspace.WorkspaceDataName);
            this.data = JsonConvert.DeserializeObject<WorkspacePrimarySettings>(this.param.Value);
        }
        public override void Save()
        {
            
        }
    }
}
