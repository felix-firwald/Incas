using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows;

namespace Incas.Templates.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для ExcelImporterSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class ExcelImporterSettings : AutoUIBase
    {
        protected override string FinishButtonText { get => "Начать анализ"; set => base.FinishButtonText = value; }
        private const string columnField = "Поле";
        private const string columnSearchName = "Имя колонки на листе";
        #region Data
        [Description("Соотношение значений")]
        public DataTable Values { get; set; }
        #endregion

        public ExcelImporterSettings(List<Objects.Models.Field> input)
        {
            this.Values = new();
            DataColumn dc = this.Values.Columns.Add(columnField);           
            this.Values.Columns.Add(columnSearchName);
            foreach (Objects.Models.Field field in input)
            {
                switch (field.Type)
                {
                    case Objects.Components.FieldType.Variable:
                    case Objects.Components.FieldType.Text:
                    case Objects.Components.FieldType.Number:
                    case Objects.Components.FieldType.LocalEnumeration:
                    case Objects.Components.FieldType.GlobalEnumeration:
                    case Objects.Components.FieldType.Date:
                        DataRow dr = this.Values.Rows.Add();
                        dr[columnField] = field.Name;
                        dr[columnSearchName] = field.VisibleName;
                        break;
                }                
            }
        }

        #region Functionality
        public Dictionary<string, string> GetMap()
        {
            Dictionary<string, string> result = new();
            foreach (DataRow dr in this.Values.Rows)
            {
                result.Add(dr[columnField].ToString(), dr[columnSearchName].ToString());
            }
            return result;
        }
        public override void Load()
        {
            
        }

        public override void Validate()
        {

        }

        public override void Save()
        {
            
        }
        #endregion
    }
}
