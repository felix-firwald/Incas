using Incas.Core.Classes;
using Incas.DialogSimpleForm.Views.Windows;

namespace Incas.DialogSimpleForm.Components
{
    public abstract class StaticAutoUIBase
    {
        private const string DefaultFinishButtonText = "Сохранить";
        private const string DefaultCancelButtonText = "Отменить";
        protected virtual string FinishButtonText { get; set; }
        protected virtual string CancelButtonText { get; set; }
        public string GetFinishButtonText()
        {
            if (string.IsNullOrEmpty(this.FinishButtonText))
            {
                return DefaultFinishButtonText;
            }
            return this.FinishButtonText;
        }
        public string GetCancelButtonText()
        {
            if (string.IsNullOrEmpty(this.CancelButtonText))
            {
                return DefaultCancelButtonText;
            }
            return this.CancelButtonText;
        }
        /// <summary>
        /// Показывает диалоговое окно, используя систему процедурной генерации форм DialogSimpleForm
        /// </summary>
        /// <param name="title"></param>
        /// <param name="icon"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public bool ShowDialog(string title, Icon icon, IconColor color)
        {
            Views.Windows.DialogSimpleForm ds = new(this, title, icon, color);
            return (bool)ds.ShowDialog();
        }
        /// <summary>
        /// Показывает диалоговое окно, используя систему процедурной генерации форм DialogSimpleForm
        /// </summary>
        /// <param name="title"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        public bool ShowDialog(string title, Icon icon)
        {
            Views.Windows.DialogSimpleForm ds = new(this, title, icon);
            return (bool)ds.ShowDialog();
        }
        /// <summary>
        /// Показывает диалоговое окно, используя систему процедурной генерации форм DialogSimpleForm
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public bool ShowDialog(string title)
        {
            Views.Windows.DialogSimpleForm ds = new(this, title);
            return (bool)ds.ShowDialog();
        }
        /// <summary>
        /// Загружает данные (вызывается перед началом загрузки формы)
        /// </summary>
        public virtual void Load()
        {

        }
        /// <summary>
        /// Валидация формы на предмет ошибок заполнения, если есть ошибки, метод должен выбрасывать SimpleFormFailed
        /// </summary>
        public virtual void Validate()
        {

        }

        /// <summary>
        /// Сохраняет данные
        /// </summary>
        public virtual void Save()
        {

        }
    }
}
