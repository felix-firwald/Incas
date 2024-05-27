using Incubator_2.ViewModels.VMAdmin;
using Incas.Users.Models;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incubator_2.Forms.AdminPanel
{
    /// <summary>
    /// Логика взаимодействия для SessionElement.xaml
    /// </summary>
    public partial class SessionElement : UserControl
    {
        private VM_SessionElement vm;
        public SessionElement(SSession session)
        {
            this.InitializeComponent();
            this.vm = new(ref session);
            this.DataContext = this.vm;
        }

        private void TerminateClick(object sender, MouseButtonEventArgs e)
        {
            this.vm.Terminate();
        }

        private void RestartClick(object sender, MouseButtonEventArgs e)
        {
            this.vm.Restart();
        }

        private void ShowExplicitClick(object sender, MouseButtonEventArgs e)
        {
            this.vm.ShowExplicit();
        }
    }
}
