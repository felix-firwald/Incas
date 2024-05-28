using Incas.Core.ViewModels;
using System.Windows;
using System.Windows.Forms;

namespace Incas.Core.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateWorkspace.xaml
    /// </summary>
    public partial class CreateWorkspace : Window
    {
        private CreateWorkspaceViewModel vm;
        public CreateWorkspace()
        {
            InitializeComponent();
            this.vm = new CreateWorkspaceViewModel();
            this.DataContext = this.vm;
        }

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            this.vm.RunInitializing();
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
