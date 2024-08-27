using Incas.Core.AutoUI;
using Incas.Core.Classes;
using Incas.Core.Models;
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
        [Description("Имя рабочего пространства")]
        public string Name
        {
            get
            {
                return ProgramState.GetParameter(ParameterType.INCUBATOR, "ws_name").value;
            }
            set
            {
                using (Parameter p = new())
                {
                    p.GetParameter(ParameterType.INCUBATOR, "ws_name");
                    p.value = value;
                    p.UpdateValue();
                }
            }
        }

        [Description("Рабочее пространство открыто")]
        public bool WorkspaceOpened
        {
            get
            {
                return ProgramState.GetParameter(ParameterType.INCUBATOR, "ws_opened").GetValueAsBool();
            }
            set
            {
                using (Parameter p = new())
                {
                    p.GetParameter(ParameterType.INCUBATOR, "ws_opened").GetValueAsBool();
                    p.WriteBoolValue(value);
                    p.UpdateValue();                   
                }
            }
        }

        [Description("Рабочее пространство заблокировано")]
        public bool WorkspaceLocked
        {
            get
            {
                return ProgramState.GetParameter(ParameterType.INCUBATOR, "ws_locked").GetValueAsBool();
            }
            set
            {
                using (Parameter p = new())
                {
                    p.GetParameter(ParameterType.INCUBATOR, "ws_locked").GetValueAsBool();
                    p.WriteBoolValue(value);
                    p.UpdateValue();
                }
            }
        }
    }
}
