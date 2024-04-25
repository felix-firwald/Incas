using Incubator_2.ViewModels;
using System.Windows;
using System.Windows.Forms;

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

        private void DefinePathClick(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.RootFolder = System.Environment.SpecialFolder.MyDocuments;
            if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.vm.WorkspacePath = fb.SelectedPath;
            }
        }
    }
}
