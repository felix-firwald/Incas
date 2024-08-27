using Incas.Core.Classes;
using System.ComponentModel;

namespace Incas.Core.AutoUI
{
    internal class SetPassword : AutoUIBase
    {
        [Description("Пароль для входа")]
        public string Password { get; set; }

        public void Save()
        {
            UserParameters parameters = ProgramState.CurrentUser.GetParametersContext();
            parameters.password = this.Password;
            ProgramState.CurrentUserParameters = parameters;
            ProgramState.CurrentUser.SaveParametersContext(parameters);
            ProgramState.CurrentUser.SaveUser();
        }
    }
}
