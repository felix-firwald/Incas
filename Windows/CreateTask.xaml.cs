using Common;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
