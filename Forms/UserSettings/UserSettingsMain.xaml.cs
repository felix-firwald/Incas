using Incubator_2.Windows;
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

namespace Incubator_2.Forms.UserSettings
{
    /// <summary>
    /// Логика взаимодействия для UserSettingsMain.xaml
    /// </summary>
    public partial class UserSettingsMain : UserControl
    {
        public UserSettingsMain()
        {
            InitializeComponent();
        }

        private void SetPasswordClick(object sender, RoutedEventArgs e)
        {
            SetPassword sp = new();
            sp.ShowDialog();
        }
    }
}
