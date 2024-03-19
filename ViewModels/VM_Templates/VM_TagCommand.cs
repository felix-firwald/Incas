using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.ViewModels.VM_Templates
{
    public class VM_TagCommand : VM_Base
    {
        private CommandSettings commandSettings;
        public VM_TagCommand(CommandSettings cs)
        {
            commandSettings = cs;
        }
        public string Name
        {
            get
            {
                if (commandSettings.Name == null)
                {
                    commandSettings.Name = "";
                }
                return commandSettings.Name;
            }
            set
            {
                commandSettings.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string Script
        {
            get
            {
                if (commandSettings.Script == null)
                {
                    commandSettings.Script = "";
                }
                return commandSettings.Script;
            }
            set
            {
                commandSettings.Script = value;
                OnPropertyChanged(nameof(Script));
            }
        }
        public string Icon
        {
            get
            {
                if (commandSettings.Icon == null)
                {
                    commandSettings.Icon = "";
                }
                return commandSettings.Icon;
            }
            set
            {
                commandSettings.Icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }
        public int ScriptType
        {
            get
            {
                return (int)commandSettings.ScriptType;
            }
            set
            {
                commandSettings.ScriptType = (ScriptType)value;
                OnPropertyChanged(nameof(ScriptType));
            }
        }
        public CommandSettings GetData()
        {
            return commandSettings;
        }
    }
}
