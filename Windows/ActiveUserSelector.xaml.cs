using Common;
using Incubator_2.ViewModels;
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
using System.Windows.Shapes;

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
