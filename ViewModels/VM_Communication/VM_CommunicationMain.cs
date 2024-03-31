using Models;
using System.Collections.Generic;

namespace Incubator_2.ViewModels.VM_Communication
{
    public class VM_CommunicationMain : VM_Base
    {
        private User _selectedUser;
        public VM_CommunicationMain() { }

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
                return _selectedUser;
            }
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
            }
        }
    }
}
