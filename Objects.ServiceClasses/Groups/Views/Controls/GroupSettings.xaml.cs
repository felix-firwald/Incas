using Incas.Objects.Engine;
using Incas.Objects.ServiceClasses.Groups.Components;
using Incas.Objects.ServiceClasses.Groups.ViewModels;
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
            return this.TargetObject;
        }
    }
}
