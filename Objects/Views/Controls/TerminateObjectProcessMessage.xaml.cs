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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Incas.Objects.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для TerminateObjectProcessMessage.xaml
    /// </summary>
    public partial class TerminateObjectProcessMessage : UserControl
    {
        public delegate void Action();
        public event Action OnTerminateRequested;
        public TerminateObjectProcessMessage()
        {
            this.InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            this.Icon.Visibility = Visibility.Collapsed;
            this.Text.Visibility = Visibility.Collapsed;
            this.OnTerminateRequested?.Invoke();
        }
    }
}
