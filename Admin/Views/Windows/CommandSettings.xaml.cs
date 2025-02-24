using Incas.Admin.ViewModels;
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

namespace Incas.Admin.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для CommandSettings.xaml
    /// </summary>
    public partial class CommandSettings : Window
    {
        private CommandsSettingsViewModel vm;
        public CommandSettings()
        {
            this.InitializeComponent();
            this.vm = new();
            this.DataContext = vm;
        }
        private void AddCommandClick(object sender, RoutedEventArgs e)
        {

        }

        private void MinimizeAllClick(object sender, RoutedEventArgs e)
        {

        }

        private void MaximizeAllClick(object sender, RoutedEventArgs e)
        {

        }

        private void GetMoreInfoClick(object sender, RoutedEventArgs e)
        {

        }
        private void SaveClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
