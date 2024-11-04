using Incas.DialogSimpleForm.Components;
using Incas.Core.Classes;
using Incas.Users.Models;
using System.ComponentModel;

namespace Incas.Users.AutoUI
{
    internal class UserSuperAdminSettings : AutoUIBase
    {
        private User _user;
        private UserParameters _userParameters = new();

        [Description("Фамилия")]
        public string Surname
        {
            get => this._user.surname;
            set => this._user.surname = value;
        }

        [Description("Имя и отчество")]
        public string SecondName
        {
            get => this._user.secondName;
            set => this._user.secondName = value;
        }

        [Description("Должность")]
        public string Post
        {
            get => this._user.post;
            set => this._user.post = value;
        }

        public UserSuperAdminSettings(User user)
        {
            this._user = user;
        }
        public override void Save()
        {
            this._user.SaveUser();
        }
    }
}
