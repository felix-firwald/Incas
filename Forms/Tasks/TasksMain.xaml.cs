using Incubator_2.ViewModels.VM_Tasks;
using System.Windows.Controls;

namespace Incubator_2.Forms.Tasks
{
    /// <summary>
    /// Логика взаимодействия для TasksMain.xaml
    /// </summary>
    public partial class TasksMain : UserControl
    {
        public VM_TasksMain vm;
        public TasksMain()
        {
            InitializeComponent();
            vm = new();
            this.DataContext = vm;
        }
        //public void UpdateList()
        //{
        //    using (Task t = new())
        //    {
        //        t.GetAllTasks().ForEach(task =>
        //        {
        //            this.ActualTasks.Children.Add(new TaskElement(task));
        //        });
        //    }
        //}
    }
}
