using Incas.DialogSimpleForm.Components;
using Incas.Miniservices.Clipboard.Classes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Incas.Miniservices.Clipboard.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для ClipboardRecord.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class ClipboardRecord : StaticAutoUIBase
    {
        #region Data
        private ClipboardRecordOld old = new();

        [StringLength(40)]
        [Description("Наименование записи")]
        public string Name { get; set; }

        [StringLength(800)]
        [Description("Текст для копирования")]
        public string Text { get; set; }
        #endregion

        #region Functionality
        public override void Load()
        {
            this.old.Name = this.Name;
            this.old.Text = this.Text;
        }
        public override void Save()
        {
            if (!string.IsNullOrEmpty(this.old.Name)) // если old пустое значит мы создаем запись а не редачим
            {
                ClipboardManager.UpdateInClipboard(this.old, this);
            }
            else
            {
                ClipboardManager.AddToClipboard(this);
            }
        }
        #endregion
    }
}
