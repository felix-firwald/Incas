
using Incubator_2.ViewModels.VMAdmin;
using Models;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для UserElement.xaml
    /// </summary>
    public partial class UserElement : UserControl
    {
        private VM_UserElement vm;
        public delegate void Deleted(UserElement element);
        public event Deleted OnDeleted;
        public UserElement(User user)
        {
            InitializeComponent();
            vm = new(user);
            this.DataContext = vm;
        }

        private void RemoveClick(object sender, MouseButtonEventArgs e)
        {
            vm.RemoveUser();
            OnDeleted?.Invoke(this);
        }

        private void EditClick(object sender, MouseButtonEventArgs e)
        {
            vm.EditUser();
        }
    }
}
