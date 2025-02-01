using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Incas.DialogSimpleForm.Exceptions;
using Incas.DialogSimpleForm.Attributes;

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
