using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using IncasEngine.Models;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using System.Collections.Generic;
using System.ComponentModel;

namespace Incas.Objects.Documents.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для PropertyGlobalConstantSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class PropertyGlobalConstantSettings : StaticAutoUIBase
    {
        private TemplateProperty Source { get; set; }
        #region Data
        [Description("Глобальная константа")]
        public Selector Selector { get; set; }
        #endregion

        public PropertyGlobalConstantSettings(TemplateProperty prop)
        {
            this.Source = prop;
            Parameter p = new();
            this.Selector = new([]);
            foreach (KeyValuePair<System.Guid, string> pair in p.GetConstantsDictionary())
            {
                this.Selector.Pairs.Add(pair.Key, pair.Value);
            }
            try
            {
                this.Selector.SetSelection(System.Guid.Parse(prop.Value));
            }
            catch
            {

            }
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
            this.Source.Value = this.Selector.SelectedObject.ToString();
        }
        #endregion
    }
}
