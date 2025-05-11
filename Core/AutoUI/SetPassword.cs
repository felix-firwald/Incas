using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using System.ComponentModel;

namespace Incas.Core.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для SetPassword.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    internal class SetPassword : StaticAutoUIBase
    {
        #region Data
        [Description("Пароль для входа")]
        public string Password { get; set; }
        #endregion

        #region Functionality
        public override void Save()
        {
            
        }
        #endregion
    }
}