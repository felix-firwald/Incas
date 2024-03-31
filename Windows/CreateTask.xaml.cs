using Common;
using Models;
using System.Windows;
using Task = Models.Task;

namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateTask.xaml
    /// </summary>
    public partial class CreateTask : Window
    {
        public CreateTask()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Task task = new();
            task.name = this.TaskName.Text;
            task.text = this.TaskDescription.Text;
            task.subtasks = new();
            ProgramState.ShowInfoDialog(this.DateFinished.SelectedDate.Value.Date.ToString());
            task.dateFinished = this.DateFinished.SelectedDate.Value.Date;
            foreach (string item in this.Subtasks.Text.Split(";"))
            {
                SSubTask sst = new SSubTask();
                sst.text = item;
                task.subtasks.Add(sst);
            }
            task.SaveTask();
        }
    }
}
