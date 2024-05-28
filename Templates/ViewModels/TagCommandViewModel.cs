using Incas.Core.ViewModels;
using Incas.Templates.Components;
using System.Windows;

namespace Incas.Templates.ViewModels
{
    public class TagCommandViewModel : BaseViewModel
    {
        private CommandSettings commandSettings;
        public TagCommandViewModel(CommandSettings cs)
        {
            this.commandSettings = cs;
        }
        public string Name
        {
            get
            {
                this.commandSettings.Name ??= "";
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
                this.commandSettings.Script ??= "";
                return this.commandSettings.Script;
            }
            set
            {
                this.commandSettings.Script = value;
                this.OnPropertyChanged(nameof(this.Script));
            }
        }
        public Visibility IconVisibility => this.ScriptType == 1 ? Visibility.Visible : Visibility.Collapsed;
        public int Icon
        {
            get => (int)this.commandSettings.Icon;
            set
            {
                this.commandSettings.Icon = (IconType)value;
                this.OnPropertyChanged(nameof(this.Icon));
            }
        }
        public int ScriptType
        {
            get => (int)this.commandSettings.ScriptType;
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
