using Incas.Core.AutoUI;
using Incas.Objects.Components;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для VirtualFieldsAppender.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class VirtualFieldsAppender : AutoUIBase
    {
        #region Data
        [Description("Наименование константы")]
        [StringLength(25)]
        public string Name { get; set; }

        [Description("Значение константы")]
        [StringLength(1200)]
        public string Text { get; set; }
        #endregion

        public VirtualFieldsAppender()
        {

        }

        #region Functionality
        public List<Models.Field> GetFields()
        {
            List<Models.Field> fields = [];
            Models.Field main = new()
            {
                Name = this.Name.Replace(" ", "_"),
                VisibleName = this.Name.Replace("_", " "),
                Value = this.Text,
                Type = FieldType.LocalConstant
            };
            fields.Add(main);
            Regex regex = new(@"\[[A-Za-zА-Яа-я0-9_]*\]");
            MatchCollection matches = regex.Matches(this.Text);

            foreach (Match match in matches)
            {
                string name = match.Value.TrimStart('[').TrimEnd(']');
                Models.Field field = new()
                {
                    Name = name,
                    VisibleName = name.Replace("_", " ")
                };
                fields.Add(field);
            }
            return fields;
        }
        #endregion
    }
}
