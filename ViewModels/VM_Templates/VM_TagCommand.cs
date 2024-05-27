using Incas.Core.ViewModels;
using Incas.Templates.Components;
using System.Windows;

namespace Incubator_2.ViewModels.VM_Templates
{
    public class VM_TagCommand : BaseViewModel
    {
        private CommandSettings commandSettings;
        public VM_TagCommand(CommandSettings cs)
        {
            this.commandSettings = cs;
        }
        public string Name
        {
            get
            {
                if (this.commandSettings.Name == null)
                {
                    this.commandSettings.Name = "";
                }
                return this.commandSettings.Name;
            }
            set
            {
                this.commandSettings.Name = value;
                this.OnPropertyChanged(nameof(this.Name));
            }
        }
        public string Script
        {
            get
            {
                if (this.commandSettings.Script == null)
                {
                    this.commandSettings.Script = "";
                }
                return this.commandSettings.Script;
            }
            set
            {
                this.commandSettings.Script = value;
                this.OnPropertyChanged(nameof(this.Script));
            }
        }
        public Visibility IconVisibility
        {
            get
            {
                if (this.ScriptType == 1)
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
                return (int)this.commandSettings.Icon;
            }
            set
            {
                this.commandSettings.Icon = (IconType)value;
                this.OnPropertyChanged(nameof(this.Icon));
            }
        }
        public int ScriptType
        {
            get
            {
                return (int)this.commandSettings.ScriptType;
            }
            set
            {
                this.commandSettings.ScriptType = (ScriptType)value;
                this.OnPropertyChanged(nameof(this.ScriptType));
                this.OnPropertyChanged(nameof(this.IconVisibility));
            }
        }

        public CommandSettings GetData()
        {
            return this.commandSettings;
        }
    }
}
