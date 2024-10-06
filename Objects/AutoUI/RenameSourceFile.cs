using Incas.DialogSimpleForm.Components;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для RenameSourceFile.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class RenameSourceFile : AutoUIBase
    {
        #region Data
        private string result { get; set; }
        [StringLength(35)]
        [Description("Имя файла без расширения")]
        public string Name { get; set; }
        #endregion

        public RenameSourceFile(string result)
        {
            this.result = result;
            this.Name = result.Replace(".docx", "").Replace(".xlsx", "");
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
            this.Name = this.Name.Replace(".docx", "").Replace(".xlsx", "");
            this.Name = this.result.Contains(".xlsx") ? (this.Name + ".xlsx") : (this.Name + ".docx");
        }
        #endregion
    }
}
