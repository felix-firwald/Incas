using Common;
using Incubator_2.ViewModels;
using Incas.Users.Models;
using System.Windows;
using Incas.Users.ViewModels;

namespace Incas.Users.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для ActiveUserSelector.xaml
    /// </summary>
    public partial class ActiveUserSelector : Window
    {
        private ActiveUserSelectorViewModel vm;
        public Session SelectedSession;
        public ActiveUserSelector(string HelpText)
        {
            this.InitializeComponent();
            this.vm = new();
            this.DataContext = this.vm;
            this.vm.HelpTextTitle = HelpText;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.vm.SelectedSession != null)
            {
                this.SelectedSession = this.vm.SelectedSession;
                this.Close();
            }
            else
            {
                ProgramState.ShowExclamationDialog("Пользователь не выбран!", "Действие невозможно");
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
