using Incas.Core.Attributes;
using Incas.Core.Classes;
using System.ComponentModel;

namespace Incas.Core.AutoUI
{
    internal class DefineExistingWorkspace : AutoUIBase
    {
        [Description("Имя в списке")]
        public string Name { get; set; }

        [Description("Путь к рабочему пространству")]
        [UrlRequired]
        public string Path { get; set; }

        public void Save()
        {
            RegistryData.SetWorkspacePath(this.Name, this.Path);
        }
    }
}
