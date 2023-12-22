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
        public VM_OpenWorkspace() 
        {

        }
        public List<string> Workspaces
        {
            get { return RegistryData.GetWorkspaces(); }
            set
            {
                OnPropertyChanged(nameof(Workspaces));
            }
        }

    }
}
