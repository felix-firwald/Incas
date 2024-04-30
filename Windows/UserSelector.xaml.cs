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
        private VM_UserSelector vm;
        public User SelectedUser { get { return this.vm.SelectedUser; } }
        public UserSelector()
        {
            InitializeComponent();
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
