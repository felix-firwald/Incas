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
    /// Логика взаимодействия для OpenIncubator.xaml
    /// </summary>
    public partial class OpenIncubator : Window
    {
        public OpenIncubator()
        {
            InitializeComponent();
            
        }

        private void LogInClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void PathReview(object sender, RoutedEventArgs e)
        {

        }
    }
}
