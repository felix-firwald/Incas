using Common;
using Incubator_2.Common;
using System.Windows;

namespace Incas.Users.Views.Windows
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
            ProgramState.ShowInfoDialog(ProgramState.CurrentUser.fullname);
            UserParameters parameters = ProgramState.CurrentUser.GetParametersContext();
            ProgramState.ShowInfoDialog(parameters.permission_group);
            parameters.password = this.Input.Text;
            ProgramState.CurrentUserParameters = parameters;
            ProgramState.CurrentUser.SaveParametersContext(parameters);
            ProgramState.CurrentUser.SaveUser();
            this.Close();
        }
    }
}
