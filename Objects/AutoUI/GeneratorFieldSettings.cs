using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using Incas.Objects.Models;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для GeneratorFieldSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class GeneratorFieldSettings : AutoUIBase
    {
        #region Data
        private Objects.Models.Field Source;
        [Description("Выбор генератора")]
        public Selector Selector { get; set; }

        [Description("Обязательно для заполнения")]
        public bool NotNull { get; set; }
        #endregion

        public GeneratorFieldSettings(Objects.Models.Field f)
        {
            this.Source = f;
            this.Selector = new(new());
            using (Class cl = new())
            {
                //foreach (Class generator in cl.GetGenerators())
                //{
                //    this.Selector.Pairs.Add(generator.identifier, generator.name);
                //}
            }
            try
            {
                this.Selector.SetSelection(System.Guid.Parse(f.Value));
            }
            catch
            {

            }
            this.NotNull = f.NotNull;
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
            this.Source.NotNull = this.NotNull;
            this.Source.Value = this.Selector.SelectedObject.ToString();
        }
        #endregion
    }
}
