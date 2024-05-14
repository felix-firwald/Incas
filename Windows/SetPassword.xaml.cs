using Common;
using Incubator_2.Common;
using System.Windows;

namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для SetPassword.xaml
    /// </summary>
    public partial class SetPassword : Window
    {
        public SetPassword()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UserParameters parameters = ProgramState.CurrentUserParameters;
            parameters.password = this.Input.Text;
            ProgramState.CurrentUserParameters = parameters;
            ProgramState.CurrentUser.SaveUser();
            this.Close();
        }
    }
}
