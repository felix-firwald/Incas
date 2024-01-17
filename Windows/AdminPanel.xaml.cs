using Common;
using Incubator_2.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;

namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel : Window
    {
        VM_AdminPanel vm;
        public AdminPanel()
        {
            InitializeComponent();
            this.vm = new VM_AdminPanel();
            this.DataContext = vm;
            if (!vm.IsAdmin)
            {
                this.rbWorkspace.Visibility = Visibility.Collapsed;
                this.rbUsers.Visibility = Visibility.Collapsed;
            }
            ProgramState.ShowWaitCursor(false);
        }

        private void CloseClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
            GC.Collect();
        }
    }
}
