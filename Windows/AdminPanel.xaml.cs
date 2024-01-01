using Incubator_2.ViewModels;
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
        }

        private void CloseClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
