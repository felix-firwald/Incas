using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using Incas.Scripting.Interfaces;
using IncasEngine.Core;
using IncasEngine.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace Incas.Scripting.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для ScriptHelperGetConstant.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class ScriptHelperGetConstant : StaticAutoUIBase, IScriptHelper
    {
        protected override string FinishButtonText { get => "Вставить код"; }
        #region Data
        [Description("Выбранная константа")]
        public Selector Constant { get; set; }

        [CanBeNull]
        [Description("Имя переменной")]
        public string VariableName { get; set; }
        #endregion

        public ScriptHelperGetConstant()
        {
            Dictionary<object, string> constants = new();
            using (Parameter p = new())
            {
                foreach (ParameterItem pi in p.GetConstants())
                {
                    constants.Add(pi, pi.Name);
                }
            }
            this.Constant = new(constants);
        }

        #region Functionality
        public override void Load()
        {

        }

        public override void Validate()
        {

        }

        public override void Save()
        {

        }

        public string GetScript()
        {
            ParameterItem item = (ParameterItem)this.Constant.SelectedObject;
            if (string.IsNullOrWhiteSpace(this.VariableName))
            {
                return $"incas.get_constant('{item.Name}')";
            }
            return $"{this.VariableName.Replace(" ", "_")} = incas.get_constant('{item.Name}')";
        }
        #endregion
    }
}
