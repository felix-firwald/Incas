using Incas.Core.ViewModels;
using Incas.Users.Models;
using System.Collections.Generic;

namespace Incas.Users.ViewModels
{
    public class UserSelectorViewModel : BaseViewModel
    {
        private User _selected;
        public UserSelectorViewModel() { }

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
