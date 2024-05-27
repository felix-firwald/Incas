using Incas.Core.ViewModels;
using Models;
using System.Collections.Generic;

namespace Incubator_2.ViewModels
{
    public class VM_Task : BaseViewModel
    {
        private STask _task;
        public VM_Task(STask task)
        {
            this._task = task;
        }
        public string TaskName
        {
            get
            {
                return this._task.name;
            }
            set
            {
                this._task.name = value;
                this.OnPropertyChanged(nameof(this.TaskName));
            }
        }

        public string TaskText
        {
            get
            {
                return this._task.text;
            }
        }

        public int SubtasksPassedCount
        {
            get
            {
                int counter = 0;
                foreach (SSubTask task in this._task.subtasks)
                {
                    if (task.passed)
                    {
                        counter++;
                    }
                }
                return counter;
            }
            set
            {
                this.OnPropertyChanged(nameof(this.SubtasksPassedCount));
            }
        }
        public int SubtasksCount
        {
            get
            {
                return this._task.subtasks.Count;
            }
        }
        public List<SSubTask> Subtasks
        {
            get
            {
                return this._task.subtasks;
            }
            set
            {
                this._task.subtasks = value;
                this.OnPropertyChanged(nameof(this.Subtasks));
            }
        }

        public void SaveChanges()
        {

            this._task.AsModel().UpdateSubtasks();
            this.OnPropertyChanged(nameof(this.SubtasksPassedCount));
        }
    }
}
