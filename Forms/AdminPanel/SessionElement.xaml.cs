using Incubator_2.ViewModels.VMAdmin;
using Models;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incubator_2.Forms.AdminPanel
{
    /// <summary>
    /// Логика взаимодействия для SessionElement.xaml
    /// </summary>
    public partial class SessionElement : UserControl
    {
        VM_SessionElement vm;
        public SessionElement(SSession session)
        {
            InitializeComponent();
            vm = new(ref session);
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
