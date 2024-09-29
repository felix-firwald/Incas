using Incas.Core.AutoUI;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для TemplateClassSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class TemplateClassSettings : AutoUIBase
    {
        #region Data
        [Description("Имя")]
        public string Name { get; set; }

        [Description("Путь к файлу")]
        [Incas.Core.Attributes.UrlRequired]
        public string Path { get; set; }
        #endregion

        public TemplateClassSettings(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }

        #region Functionality

        #endregion
    }
}
