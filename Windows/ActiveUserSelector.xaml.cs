using Common;
using Incubator_2.ViewModels;
using Models;
using System.Windows;

namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для ActiveUserSelector.xaml
    /// </summary>
    public partial class ActiveUserSelector : Window
    {
        VM_ActiveUserSelector vm;
        public Session SelectedSession;
        public ActiveUserSelector(string HelpText)
        {
            InitializeComponent();
            this.vm = new();
            this.DataContext = vm;
            this.vm.HelpTextTitle = HelpText;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (vm.SelectedSession != null)
            {
                SelectedSession = vm.SelectedSession;
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
