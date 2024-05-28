using Common;
using Incas.Core.ViewModels;
using System.IO;
using System.Windows;

namespace Incas.Core.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для DefineExistingWorkspace.xaml
    /// </summary>
    public partial class DefineExistingWorkspace : Window
    {
        private DefExistWorkspaceViewModel vm;
        public DefineExistingWorkspace(DefExistWorkspaceViewModel vmedit = null)
        {
            this.InitializeComponent();

            if (this.vm != null)
            {
                this.DataContext = vmedit;
                this.name.IsEnabled = false;
            }
            else
            {
                this.vm = new DefExistWorkspaceViewModel();
                this.DataContext = this.vm;
            }

        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (this.name.Text.Length < 5)
            {
                ProgramState.ShowErrorDialog("Имя слишком короткое!", "Сохранение прервано");
                return;
            }
            if (!Directory.Exists(this.path.Text))
            {
                ProgramState.ShowErrorDialog("Такого пути не существует!", "Сохранение прервано");
                return;
            }
            if (!File.Exists($"{this.path.Text}\\data.dbinc"))
            {
                ProgramState.ShowErrorDialog("Рабочее пространство не найдено.", "Сохранение прервано");
                return;
            }
            RegistryData.SetWorkspacePath(this.vm.WorkspaceName, this.vm.WorkspacePath);
            this.Close();
        }
    }
}
