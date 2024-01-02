using Incubator_2.ViewModels.VMAdmin;
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

namespace Incubator_2.Forms.AdminPanel
{
    /// <summary>
    /// Логика взаимодействия для SessionElement.xaml
    /// </summary>
    public partial class SessionElement : UserControl
    {
        VM_SessionElement vm;
        public SessionElement(Session session)
        {
            InitializeComponent();
            vm = new(session);
            this.DataContext = vm;
        }

        private void TerminateClick(object sender, MouseButtonEventArgs e)
        {
            vm.Terminate();
        }

        private void RestartClick(object sender, MouseButtonEventArgs e)
        {
            vm.Restart();
        }

        private void ShowExplicitClick(object sender, MouseButtonEventArgs e)
        {
            vm.ShowExplicit();
        }
    }
}
