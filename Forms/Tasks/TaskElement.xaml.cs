using Incubator_2.ViewModels;
using Models;
using System.Windows.Controls;

namespace Incubator_2.Forms.Tasks
{
    /// <summary>
    /// Логика взаимодействия для TaskElement.xaml
    /// </summary>
    public partial class TaskElement : UserControl
    {
        VM_Task vm;
        public TaskElement(STask task)
        {
            InitializeComponent();
            this.vm = new(task);
            this.DataContext = vm;
        }

        private void CheckClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            vm.SaveChanges();
        }
    }
}
