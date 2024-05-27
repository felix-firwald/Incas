using Incas.Core.ViewModels;
using Models;
using System.Collections.Generic;

namespace Incubator_2.ViewModels
{
    public class VM_UserSelector : BaseViewModel
    {
        private User _selected;
        public VM_UserSelector() { }

        public List<User> Users
        {
            get
            {
                using User u = new();
                return u.GetAllUsers();
            }
        }
        public User SelectedUser
        {
            get
            {
                return this._selected;
            }
            set
            {
                this._selected = value;
                this.OnPropertyChanged(nameof(this.SelectedUser));
            }
        }
    }
}
