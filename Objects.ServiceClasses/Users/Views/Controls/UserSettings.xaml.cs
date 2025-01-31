using Incas.Objects.Engine;
using Incas.Objects.ServiceClasses.Groups.ViewModels;
using Incas.Objects.ServiceClasses.Users.Components;
using Incas.Objects.ServiceClasses.Users.ViewModels;
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
            return this.TargetObject;
        }
    }
}
