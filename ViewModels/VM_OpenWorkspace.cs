using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.AtomPub;

namespace Incubator_2.ViewModels
{
    class VM_OpenWorkspace : VM_Base
    {
        private List<string> _workspace = RegistryData.GetWorkspaces();
        private string _selectedWorkspace;
        public VM_OpenWorkspace() 
        {

        }
        public List<string> Workspaces
        {
            get { return _workspace; }
            set
            {
                _workspace = value;
                OnPropertyChanged(nameof(Workspaces));
            }
        }
        public string SelectedWorkspace
        {
            get
            {
                return _selectedWorkspace;
            }
            set
            {
                _selectedWorkspace = value;
                OnPropertyChanged(nameof(SelectedWorkspace));
                OnPropertyChanged(nameof(Path));
            }
        }
        public string Path
        {
            get
            {
                return RegistryData.GetWorkspacePath(_selectedWorkspace);
            }
            set
            {
                RegistryData.SetWorkspacePath(_selectedWorkspace, value);
                OnPropertyChanged(nameof(Path));
            }
        }
    }
}
