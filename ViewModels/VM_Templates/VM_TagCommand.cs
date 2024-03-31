using Models;
using System.Windows;

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
        public Visibility IconVisibility
        {
            get
            {
                if (ScriptType == 1)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        public int Icon
        {
            get
            {
                return (int)commandSettings.Icon;
            }
            set
            {
                commandSettings.Icon = (IconType)value;
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
                OnPropertyChanged(nameof(IconVisibility));
            }
        }

        public CommandSettings GetData()
        {
            return commandSettings;
        }
    }
}
