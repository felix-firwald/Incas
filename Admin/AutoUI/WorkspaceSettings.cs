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
        public override void Load()
        {
            this.data = JsonConvert.DeserializeObject<WorkspacePrimarySettings>(ProgramState.GetParameter(ParameterType.WORKSPACE, "ws_data").value);
        }
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
    }
}
