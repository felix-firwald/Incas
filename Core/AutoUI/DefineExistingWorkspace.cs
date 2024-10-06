using Incas.Core.Attributes;
using Incas.Core.Classes;
using System.ComponentModel;

namespace Incas.Core.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для DefineExistingWorkspace.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    internal class DefineExistingWorkspace : AutoUIBase
    {
        #region Data
        [Description("Имя в списке")]
        public string Name { get; set; }

        [Description("Путь к рабочему пространству")]
        [UrlRequired]
        public string Path { get; set; }
        #endregion

        #region Functionality
        public override void Save()
        {
            RegistryData.SetWorkspacePath(this.Name, this.Path);
        }
        #endregion
    }
}
