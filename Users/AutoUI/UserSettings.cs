using Common;
using Incas.Core.AutoUI;
using Incas.Core.Classes;
using Incas.Users.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Incas.Users.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для UserSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    internal class UserSettings : AutoUIBase
    {
        #region Data
        private User _user;
        private UserParameters _userParameters = new();

        [Description("Роль")]
        public ComboSelector Status { get; set; }

        [Description("Юзернейм")]
        public string Username
        {
            get
            {
                return this._user.username;
            }
            set
            {
                if (this._user.id == Guid.Empty)
                {
                    this._user.username = value;
                }              
            }
        }

        [Description("Фамилия")]
        public string Surname
        {
            get
            {
                return this._user.surname;
            }
            set
            {
                this._user.surname = value;
            }
        }

        [Description("Имя и отчество")]
        public string SecondName
        {
            get
            {
                return this._user.secondName;
            }
            set
            {
                this._user.secondName = value;
            }
        }

        [Description("Должность")]
        public string Post
        {
            get
            {
                return this._user.post;
            }
            set
            {
                this._user.post = value;
            }
        }

        [Description("Первоначальный пароль")]
        public string Password
        {
            get
            {
                return this._userParameters.startup_password;
            }
            set
            {
                this._userParameters.startup_password = value;
            }
        }
        #endregion

        #region Functionality
        public UserSettings(User user)
        {
            this.Status = new(
                new Dictionary<object, string>
                {
                    { PermissionGroup.Admin, "Администратор" },
                    { PermissionGroup.Moderator, "Модератор" },
                    { PermissionGroup.Editor, "Редактор" },
                    { PermissionGroup.Operator, "Оператор" }
                }
            );
            this._user = user;            
            if (this._user.id != Guid.Empty)
            {
                this._userParameters = this._user.GetParametersContext();
            }
            this.Status.SetSelection(this._userParameters.permission_group);
        }
        public void Save()
        {
            this._userParameters.permission_group = (PermissionGroup)this.Status.SelectedObject;
            this._user.SaveParametersContext(this._userParameters);
            this._user.SaveUser();
        }
        #endregion
    }

    class UserSuperAdminSettings : AutoUIBase
    {
        private User _user;
        private UserParameters _userParameters = new();

        [Description("Фамилия")]
        public string Surname
        {
            get
            {
                return this._user.surname;
            }
            set
            {
                this._user.surname = value;
            }
        }

        [Description("Имя и отчество")]
        public string SecondName
        {
            get
            {
                return this._user.secondName;
            }
            set
            {
                this._user.secondName = value;
            }
        }

        [Description("Должность")]
        public string Post
        {
            get
            {
                return this._user.post;
            }
            set
            {
                this._user.post = value;
            }
        }

        public UserSuperAdminSettings(User user)
        {
            this._user = user;
        }
        public void Save()
        {
            this._user.SaveUser();
        }
    }
}
