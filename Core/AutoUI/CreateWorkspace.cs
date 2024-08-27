using Incas.Core.Attributes;
using Incas.Core.Classes;
using System.ComponentModel;

namespace Incas.Core.AutoUI
{
    internal class CreateWorkspace : AutoUIBase
    {
        [Description("Наименование рабочего пространство")]
        public string WorkspaceName { get; set; }

        [Description("Путь к рабочему пространству")]
        [UrlRequired]
        public string Path { get; set; }

        [Description("Пароль для входа")]
        public string Password { get; set; }
    }
}
