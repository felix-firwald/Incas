
using Incas.Users.Models;
using Incas.Users.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для UserElement.xaml
    /// </summary>
    public partial class UserElement : UserControl
    {
        private UserElementViewModel vm;
        public delegate void Deleted(UserElement element);
        public event Deleted OnDeleted;
        public UserElement(User user)
        {
            this.InitializeComponent();
            this.vm = new(user);
            this.DataContext = this.vm;
        }

        private void RemoveClick(object sender, MouseButtonEventArgs e)
        {
            this.vm.RemoveUser();
            OnDeleted?.Invoke(this);
        }

        private void EditClick(object sender, MouseButtonEventArgs e)
        {
            this.vm.EditUser();
        }
    }
}
