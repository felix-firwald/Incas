using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Incas.DialogSimpleForm.Exceptions;
using Incas.DialogSimpleForm.Attributes;
using System.Collections.Generic;

namespace Incas.Objects.ServiceClasses.Users.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для PasswordChanger.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class PasswordChanger : AutoUIBase
    {
        private List<string> unprotectedSequences = [
            "1234",
            "9876",
            "0000",
            "1111",
            "2222",
            "3333",
            "4444",
            "5555",
            "6666",
            "7777",
            "8888",
            "9999",
            "qwer",
            "asdf",
            "zxcv"
            ];
        #region Data
        [MaxLength(16)]
        [PasswordAttribute]
        [Description("Придумайте пароль")]
        public string Password1 { get; set; }

        [MaxLength(16)]
        [PasswordAttribute]
        [Description("Повторите введенный пароль")]
        public string Password2 { get; set; }
        #endregion

        public PasswordChanger()
        {
            
        }

        #region Functionality
        public override void Load()
        {
            
        }

        public override void Validate()
        {
            if (this.Password1.Length < 4)
            {
                throw new SimpleFormFailed("Пароль не может быть короче 4 символов.");
            }
            if (this.unprotectedSequences.Contains(this.Password1.ToLower()))
            {
                throw new SimpleFormFailed("Пароль небезопасен. Придумайте другую комбинацию.");
            }
            if (this.Password1 != this.Password2)
            {
                throw new SimpleFormFailed("Пароли не совпадают!");
            }
        }

        public override void Save()
        {
            
        }
        #endregion
    }
}
