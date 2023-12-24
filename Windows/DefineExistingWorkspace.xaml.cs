using Common;
using Incubator_2.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для DefineExistingWorkspace.xaml
    /// </summary>
    public partial class DefineExistingWorkspace : Window
    {
        VM_DefExistWorkspace vm;
        public DefineExistingWorkspace(VM_DefExistWorkspace vmedit = null)
        {
            InitializeComponent();
            
            if (vm != null )
            {
                this.DataContext = vmedit;
                this.name.IsEnabled = false;
            }
            else
            {
                this.vm = new VM_DefExistWorkspace();
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
            RegistryData.SetWorkspacePath(vm.WorkspaceName, vm.WorkspacePath);
            this.Close();
        }
    }
}
