using Incas.Core.Classes;
using Incas.Core.ViewModels;
using IncasEngine.Workspace;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Incas.Admin.ViewModels
{
    public class ComponentsSettingsViewModel : BaseViewModel
    {
        public ComponentsSettingsViewModel()
        {
            List<WorkspaceComponent> list = ProgramState.CurrentWorkspace.GetDefinition(true).Components;

            this.Components = new();
            foreach (WorkspaceComponent c in list)
            {
                this.Components.Add(new(c));
            }
            this.SelectedComponent = this.Components[0];
        }
        private ObservableCollection<ComponentViewModel> components;
        public ObservableCollection<ComponentViewModel> Components
        {
            get
            {
                return this.components;
            }
            set
            {
                this.components = value;
                this.OnPropertyChanged(nameof(this.Components));
            }
        }
        private ComponentViewModel selected;
        public ComponentViewModel SelectedComponent
        {
            get
            {
                return this.selected;
            }
            set
            {
                this.selected = value;
                this.OnPropertyChanged(nameof(this.SelectedComponent));
            }
        }
        public void Save()
        {
            List<WorkspaceComponent> result = new();
            foreach (ComponentViewModel vm in this.Components)
            {
                if (string.IsNullOrWhiteSpace(vm.Name))
                {
                    throw new ValidationException("Одному из компонентов не присвоено имя.");
                }
                result.Add(vm.Component);
            }
            WorkspaceDefinition def = ProgramState.CurrentWorkspace.GetDefinition(true);
            def.Components = result;
            ProgramState.CurrentWorkspace.UpdateDefinition(def);
        }
    }
}
