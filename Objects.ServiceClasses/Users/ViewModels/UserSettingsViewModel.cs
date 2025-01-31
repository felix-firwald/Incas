using Incas.Core.Classes;
using Incas.Core.ViewModels;
using Incas.Objects.Engine;
using Incas.Objects.ServiceClasses.Groups.Components;
using Incas.Objects.ServiceClasses.Users.Components;
using System.Collections.Generic;

namespace Incas.Objects.ServiceClasses.Users.ViewModels
{
    public class UserSettingsViewModel : BaseViewModel, IViewModel
    {
        public User User { get; set; }
        public UserSettingsViewModel(User user)
        {
            this.User = user;
        }
        public string Password
        {
            get
            {
                return this.User.Data.Password;
            }
            set
            {
                this.User.Data.Password = value;
                this.OnPropertyChanged(nameof(this.Password));
            }
        }
         
        public List<GroupItem> Groups
        {
            get
            {
                return Group.GetItems(Processor.GetSimpleObjectsList(ProgramState.CurrentWorkspace.GetDefinition().ServiceGroups));
            }
        }
        public GroupItem SelectedGroup
        {
            get
            {
                return this.User.GetGroup().AsItem();
            }
            set
            {
                this.User.Group = value.Id;
                this.OnPropertyChanged(nameof(this.SelectedGroup));
            }
        }
        public bool GroupEditable
        {
            get
            {
                return !this.User.Data.Indestructible;
            }
        }
    }
}
