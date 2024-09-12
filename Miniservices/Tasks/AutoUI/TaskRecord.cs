using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.Core.AutoUI;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System;
using Incas.Miniservices.Clipboard.Classes;
using Incas.Miniservices.Tasks.Classes;

namespace Incas.Miniservices.Tasks.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для TaskRecord.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class TaskRecord : AutoUIBase
    {
        #region Data
        private TaskboardRecordOld old = new();
        [StringLength(40)]
        [Description("Наименование задачи")]
        public string Name { get; set; }

        [StringLength(800)]
        [Description("Текст задачи")]
        public string Text { get; set; }

        [Description("Дата напоминания")]
        public DateTime Deadline { get; set; }

        #endregion

        #region Functionality
        public void Load()
        {
            this.old.Name = this.Name;
            this.old.Text = this.Text;
            this.old.Deadline = this.Deadline;
        }
        public void Save()
        {
            if (!string.IsNullOrEmpty(this.old.Name)) // если old пустое значит мы создаем запись а не редачим
            {
                TaskManager.UpdateInTaskboard(this.old, this);
            }
            else
            {
                TaskManager.AddToTaskboard(this);
            }
        }
        #endregion
    }
}
