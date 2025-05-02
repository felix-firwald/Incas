using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using IncasEngine.Core.ExtensionMethods;
using IncasEngine.ObjectiveEngine.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для ClassFieldsString.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class ClassFieldsString : AutoUIBase
    {
        protected override string FinishButtonText { get => "Создать поля"; }
        #region Data
        [Description("Исходная строка")]
        [StringLength(1200)]
        public string Text { get; set; }
        #endregion

        public ClassFieldsString()
        {

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
        public List<Field> GetFields()
        {
            List<Field> result = new();
            string[] fieldsTextParts = this.Text.Split('\n');
            foreach (string part in fieldsTextParts)
            {
                Field f = new();
                f.SetId();
                if (part.Contains(':'))
                {
                    string[] names = part.Split(':');
                    if (names.Length > 1)
                    {
                        f.Name = names[0].Trim();
                        f.VisibleName = names[1].Trim().UppercaseFirst();
                    }
                    else if (names.Length == 1)
                    {
                        f.Name = names[0].Trim();
                        f.VisibleName = names[0].Trim().UppercaseFirst();
                    }
                }
                else
                {
                    f.Name = part.Trim();
                    f.VisibleName = part.Trim().UppercaseFirst();
                }
                result.Add(f);
            }
            return result;
        }
        #endregion
    }
}
