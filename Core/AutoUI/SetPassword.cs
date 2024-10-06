using Incas.Core.Classes;
using System.ComponentModel;

namespace Incas.Core.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для SetPassword.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    internal class SetPassword : AutoUIBase
    {
        #region Data
        [Description("Пароль для входа")]
        public string Password { get; set; }
        #endregion

        #region Functionality
        public override void Save()
        {
            UserParameters parameters = ProgramState.CurrentUser.GetParametersContext();
            parameters.password = this.Password;
            ProgramState.CurrentUserParameters = parameters;
            ProgramState.CurrentUser.SaveParametersContext(parameters);
            ProgramState.CurrentUser.SaveUser();
        }
        #endregion
    }
}