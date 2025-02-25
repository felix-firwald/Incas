using Incas.Core.ViewModels;
using IncasEngine.Workspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.ServiceClasses.Groups.ViewModels
{
    public class GroupComponentViewModel : BaseViewModel
    {
        public GroupComponentViewModel(WorkspaceComponent comp, bool activated)
        {
            this.Component = comp;
            this.IsActivated = activated;
        }
        public WorkspaceComponent Component { get; set; }
        public string Name
        {
            get
            {
                return this.Component.Name;
            }
        }
        private bool activated;
        public bool IsActivated
        {
            get
            {
                return this.activated;
            }
            set
            {
                this.activated = value;
                this.OnPropertyChanged(nameof(this.IsActivated));
            }
        }
    }
}
