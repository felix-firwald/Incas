using System.Windows;
using System.Windows.Controls;

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
