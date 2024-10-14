using Incas.Core.Models;
using Incas.DialogSimpleForm.Components;
using System.ComponentModel;

namespace Incas.Admin.AutoUI
{
    /// <summary>
    /// Логика взаимодействия для WorkspaceManager.xaml
    /// </summary>
    public class ParameterConstant : AutoUIBase
    {
        protected override string FinishButtonText { get => "Сохранить константу"; }
        private bool isEdit = false;
        [Description("Наименование константы")]
        public string Name { get; set; }

        [Description("Значение константы")]
        public string Value { get; set; }

        public ParameterConstant(bool isEdit = false)
        {
            this.isEdit = isEdit;
        }
        public override void Validate()
        {
            if (this.isEdit == false)
            {
                using Parameter p = new();
                foreach (string name in p.GetConstantsList())
                {
                    if (name == this.Name)
                    {
                        throw new DialogSimpleForm.Exceptions.SimpleFormFailed("Глобальная константа с таким наименованием уже есть в рабочем пространстве.");
                    }
                }
            }           
        }
    }
}
