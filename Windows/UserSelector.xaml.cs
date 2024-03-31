using Common;
using Incubator_2.ViewModels;
using Models;
using System.Windows;

namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для UserSelector.xaml
    /// </summary>
    public partial class UserSelector : Window
    {
        public DialogStatus Result = DialogStatus.Undefined;
        VM_UserSelector vm;
        public User SelectedUser { get { return vm.SelectedUser; } }
        public UserSelector()
        {
            InitializeComponent();
            vm = new();
            this.DataContext = vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedUser != null)
            {
                Result = DialogStatus.Yes;
                this.Close();
            }
            else
            {
                ProgramState.ShowExclamationDialog("Пользователь не выбран!", "Действие прервано");
            }
        }
    }
}
