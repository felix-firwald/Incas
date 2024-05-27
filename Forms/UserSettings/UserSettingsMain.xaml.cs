using Incas.Users.Views.Windows;
using System.Windows;
using System.Windows.Controls;

namespace Incubator_2.Forms.UserSettings
{
    /// <summary>
    /// Логика взаимодействия для UserSettingsMain.xaml
    /// </summary>
    public partial class UserSettingsMain : UserControl
    {
        public UserSettingsMain()
        {
            this.InitializeComponent();
        }

        private void SetPasswordClick(object sender, RoutedEventArgs e)
        {
            SetPassword sp = new();
            sp.ShowDialog();
        }
    }
}
