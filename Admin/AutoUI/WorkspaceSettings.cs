using Incas.Core.AutoUI;
using Incas.Core.Classes;
using Incas.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            get
            {
                return this.data.Name;
            }
            set
            {
                this.data.Name = value;
            }
        }

        [Description("Рабочее пространство заблокировано")]
        public bool WorkspaceLocked
        {
            get
            {
                return this.data.IsLocked;
            }
            set
            {
                this.data.IsLocked = value;
            }
        }
    }
}
