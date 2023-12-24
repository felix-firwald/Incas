using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.ViewModels
{
    class VM_OpenWorkspace : VM_Base
    {
        private List<string> _workspaces = RegistryData.GetWorkspaces();
        public VM_OpenWorkspace() 
        {

        }

        public void Refresh()
        {
            _workspaces = RegistryData.GetWorkspaces();
            OnPropertyChanged(nameof(Workspaces));
            OnPropertyChanged(nameof(SelectedWorkspace));
            OnPropertyChanged(nameof(Path));
            OnPropertyChanged(nameof(Password));
        }

        public void RemoveSelected()
        {
            RegistryData.RemoveWorkspace(SelectedWorkspace);
            RegistryData.SetSelectedWorkspace("");
            Refresh();
        }

        #region Public Properties
        public List<string> Workspaces
        {
            get { return _workspaces; }
            set
            {
                _workspaces = value;
                OnPropertyChanged(nameof(Workspaces));
            }
        }
        public string SelectedWorkspace
        {
            get
            {
                return RegistryData.GetSelectedWorkspace();
            }
            set
            {
                RegistryData.SetSelectedWorkspace(value);
                OnPropertyChanged(nameof(SelectedWorkspace));
                OnPropertyChanged(nameof(Path));
                OnPropertyChanged(nameof(Password));
            }
        }
        public string Path
        {
            get
            {
                return RegistryData.GetWorkspacePath(SelectedWorkspace);
            }
            set
            {
                RegistryData.SetWorkspacePath(SelectedWorkspace, value);
                OnPropertyChanged(nameof(Path));
            }
        }
        public string Password
        {
            get
            {
                return RegistryData.GetWorkspacePassword(SelectedWorkspace);
            }
            set
            {
                RegistryData.SetWorkspacePassword(SelectedWorkspace, value);
                OnPropertyChanged(nameof(Password));
            }
        }
        #endregion
    }
}
