using Models;
using System.Collections.Generic;

namespace Incubator_2.ViewModels
{
    public class VM_Task : VM_Base
    {
        private STask _task;
        public VM_Task(STask task)
        {
            _task = task;
        }
        public string TaskName
        {
            get
            {
                return _task.name;
            }
            set
            {
                _task.name = value;
                OnPropertyChanged(nameof(TaskName));
            }
        }

        public string TaskText
        {
            get
            {
                return _task.text;
            }
        }

        public int SubtasksPassedCount
        {
            get
            {
                int counter = 0;
                foreach (SSubTask task in _task.subtasks)
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
                OnPropertyChanged(nameof(SubtasksPassedCount));
            }
        }
        public int SubtasksCount
        {
            get
            {
                return _task.subtasks.Count;
            }
        }
        public List<SSubTask> Subtasks
        {
            get
            {
                return _task.subtasks;
            }
            set
            {
                _task.subtasks = value;
                OnPropertyChanged(nameof(Subtasks));
            }
        }

        public void SaveChanges()
        {

            _task.AsModel().UpdateSubtasks();
            OnPropertyChanged(nameof(SubtasksPassedCount));
        }
    }
}
