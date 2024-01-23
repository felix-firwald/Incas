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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Task = Models.Task;

namespace Incubator_2.Forms.Tasks
{
    /// <summary>
    /// Логика взаимодействия для TasksMain.xaml
    /// </summary>
    public partial class TasksMain : UserControl
    {
        public TasksMain()
        {
            InitializeComponent();
            UpdateList();
        }
        public void UpdateList()
        {
            using (Task t = new())
            {
                t.GetAllTasks().ForEach(task =>
                {
                    this.ActualTasks.Children.Add(new TaskElement(task));
                });
            }
        }
    }
}
