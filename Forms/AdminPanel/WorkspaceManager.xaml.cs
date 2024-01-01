using Incubator_2.ViewModels.AdminPanel;
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

namespace Incubator_2.Forms.AdminPanel
{
    /// <summary>
    /// Логика взаимодействия для WorkspaceManager.xaml
    /// </summary>
    public partial class WorkspaceManager : UserControl
    {
        VM_WorkspaceParameters vm;
        public WorkspaceManager()
        {
            InitializeComponent();
            this.vm = new VM_WorkspaceParameters();
            this.DataContext = vm;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            this.vm.SaveParameters();
        }
    }
}
