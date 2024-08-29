using Incas.Core.Attributes;
using Incas.Core.Classes;
using System.ComponentModel;

namespace Incas.Core.AutoUI
{
    public class CreateWorkspace : AutoUIBase
    {
        [Description("Наименование рабочего пространства")]
        public string WorkspaceName { get; set; }

        [Description("Путь к рабочему пространству")]
        [UrlRequired]
        public string Path { get; set; }

        [Description("Пароль для входа")]
        public string Password { get; set; }

        public void Save()
        {
            ProgramState.InitWorkspace(this);
        }
    }
}
