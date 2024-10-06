using DocumentFormat.OpenXml.Wordprocessing;
using Incas.Core.AutoUI;
using Incas.Core.Models;
using System.ComponentModel;

namespace Incas.Admin.AutoUI
{
    /// <summary>
    /// Логика взаимодействия для WorkspaceManager.xaml
    /// </summary>
    public class ParameterConstant : AutoUIBase
    {
        [Description("Наименование константы")]
        public string Name { get; set; }

        [Description("Значение константы")]
        public string Value { get; set; }

        public override void Validate()
        {
            using Parameter p = new();
            foreach (string name in p.GetConstantsList())
            {
                if (name == this.Name)
                {
                    throw new Core.Exceptions.SimpleFormFailed("Глобальная константа с таким наименованием уже есть в рабочем пространстве.");
                }
            }
        }
    }
}
