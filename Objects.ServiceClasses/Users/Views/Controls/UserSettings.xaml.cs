using Incas.Objects.ServiceClasses.Users.AutoUI;
using Incas.Objects.ServiceClasses.Users.ViewModels;
using IncasEngine.ObjectiveEngine.Exceptions;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Users.Components;
using System.Windows.Controls;

namespace Incas.Objects.ServiceClasses.Users.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для UserSettings.xaml
    /// </summary>
    public partial class UserSettings : UserControl, IServiceFieldFiller
    {
        public IViewModel ViewModel { get; set; }
        public IObject TargetObject { get; set; }
        public UserSettings()
        {
            this.InitializeComponent();
        }
        public IServiceFieldFiller SetUp(IObject obj)
        {
            this.TargetObject = obj;
            this.ViewModel = new UserSettingsViewModel(((User)this.TargetObject));
            this.DataContext = this.ViewModel;
            return this;
        }
        public IObject GetResult()
        {
            if (string.IsNullOrEmpty(((User)this.TargetObject).Data.Password))
            {
                throw new NotNullFailed("Не установлен пароль.");
            }
            return this.TargetObject;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PasswordChanger pc = new();
            if (pc.ShowDialog("Придумайте пароль", Core.Classes.Icon.Key))
            {
                ((User)this.TargetObject).Data.Password = pc.Password1;
            }
        }
    }
}
