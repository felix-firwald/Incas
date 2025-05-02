using Incas.Core.Classes;
using Incas.Core.ViewModels;
using IncasEngine.Workspace;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Admin.ViewModels
{
    public class CommandsSettingsViewModel : BaseViewModel
    {
        public CommandsSettingsViewModel()
        {
            foreach (WorkspaceMenuCommand command in ProgramState.CurrentWorkspace.GetDefinition().Commands)
            {
                this.AddCommand(command);
            }
        }
        private ObservableCollection<CommandViewModel> commands = new();
        public ObservableCollection<CommandViewModel> Commands
        {
            get
            {
                return this.commands;
            }
            set
            {
                this.commands = value;
                this.OnPropertyChanged(nameof(this.Commands));
            }
        }
        public void AddCommand(WorkspaceMenuCommand command)
        {
            this.Commands.Add(new(command));
        }
        private CommandViewModel selected;
        public CommandViewModel SelectedCommand
        {
            get
            {
                return this.selected;
            }
            set
            {
                this.selected = value;
                this.OnPropertyChanged(nameof(this.SelectedCommand));
            }
        }
        public void Save()
        {
            List<WorkspaceMenuCommand> result = new();
            foreach (CommandViewModel cvm in this.Commands)
            {
                result.Add(cvm.Source);
            }
            WorkspaceDefinition wd = ProgramState.CurrentWorkspace.GetDefinition(true);
            wd.Commands = result;
            ProgramState.CurrentWorkspace.UpdateDefinition(wd);
            ProgramState.MainWindow.UpdateCommands();
        }
    }
}
