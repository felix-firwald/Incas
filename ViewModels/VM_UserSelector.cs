using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.ViewModels
{
    public class VM_UserSelector : VM_Base
    {
        private User _selected;
        public VM_UserSelector() { }

        public List<User> Users
        {
            get
            {
                using (User u = new())
                {
                    return u.GetAllUsers();
                }
            }
        }
        public User SelectedUser
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                OnPropertyChanged(nameof(SelectedUser));
            }
        }
    }
}
