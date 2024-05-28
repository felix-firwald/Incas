using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Incas.Users.Models;
using Incas.Users.ViewModels;
using System.Windows;

namespace Incas.Users.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для UserSelector.xaml
    /// </summary>
    public partial class UserSelector : Window
    {
        public DialogStatus Result = DialogStatus.Undefined;
        private UserSelectorViewModel vm;
        public User SelectedUser => this.vm.SelectedUser;
        public UserSelector()
        {
            this.InitializeComponent();
            this.vm = new();
            this.DataContext = this.vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.SelectedUser != null)
            {
                this.Result = DialogStatus.Yes;
                this.Close();
            }
            else
            {
                ProgramState.ShowExclamationDialog("Пользователь не выбран!", "Действие прервано");
            }
        }
    }
}
