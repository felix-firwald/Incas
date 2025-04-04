using Incas.Core.Classes;
using Incas.Core.ViewModels;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Groups.Components;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Users.Components;
using System;
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
        //public string Password
        //{
        //    get
        //    {
        //        return this.User.Data.Password;
        //    }
        //    set
        //    {
        //        this.User.Data.Password = value;
        //        this.OnPropertyChanged(nameof(this.Password));
        //    }
        //}
         
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
                GroupItem item = this.User.GetGroup().AsItem();
                if (item.Id == Guid.Empty)
                {
                    this.User.Group = this.Groups[0].Id;
                    return this.Groups[0];
                }
                return item;
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
