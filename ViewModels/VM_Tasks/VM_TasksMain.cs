using Incas.Core.ViewModels;
using Models;
using System.Collections.Generic;

namespace Incubator_2.ViewModels.VM_Tasks
{
    public class VM_TasksMain : BaseViewModel
    {
        public VM_TasksMain() { }
        public List<Task> Tasks
        {
            get
            {
                using Task t = new();
                return t.GetAllTasksModels();
            }
            set
            {
                this.OnPropertyChanged(nameof(this.Tasks));
            }
        }
    }
}
