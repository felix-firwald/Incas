using Incubator_2.ViewModels;
using System.Windows;

namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateWorkspace.xaml
    /// </summary>
    public partial class CreateWorkspace : Window
    {
        VM_CreateWorkspace vm;
        public CreateWorkspace()
        {
            InitializeComponent();
            vm = new VM_CreateWorkspace();
            this.DataContext = vm;
        }

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            vm.RunInitializing();
            this.Close();
        }
    }
}
