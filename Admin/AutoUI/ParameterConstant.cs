using System.ComponentModel;
using DocumentFormat.OpenXml.Wordprocessing;
using Incas.Core.AutoUI;

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
    }
}
