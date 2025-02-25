using Incas.Objects.ServiceClasses.Groups.ViewModels;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Groups.Components;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Users.Components;
using System;
using System.Windows.Controls;

namespace Incas.Objects.ServiceClasses.Groups.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для GroupSettings.xaml
    /// </summary>
    public partial class GroupSettings : UserControl, IServiceFieldFiller
    {
        public IViewModel ViewModel { get; set; }
        public IObject TargetObject { get; set; }
        public GroupSettings()
        {
            this.InitializeComponent();            
        }
        public IServiceFieldFiller SetUp(IObject obj)
        {
            this.TargetObject = obj;
            Group group = (Group)this.TargetObject;
            if (obj.Id == Guid.Empty)
            {
                group.Data = new();
            }
            this.ViewModel = new GroupSettingsViewModel(group.Data);
            this.DataContext = this.ViewModel;
            return this;
        }
        public IObject GetResult()
        {
            ((GroupSettingsViewModel)this.ViewModel).ApplyCustomPermissions().ApplyComponents();
            return this.TargetObject;
        }

        private void CustomPermissionSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this.List.SelectedItem = null;
        }
    }
}
