using Incas.Core.Classes;
using Incas.Core.Views.Windows;

namespace Incas.Core.AutoUI
{
    public abstract class AutoUIBase
    {
        /// <summary>
        /// Показывает диалоговое окно, используя систему процедурной генерации форм DialogSimpleForm
        /// </summary>
        /// <param name="title"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        public bool ShowDialog(string title, Icon icon)
        {
            DialogSimpleForm ds = new(this, title, icon);
            return (bool)ds.ShowDialog();
        }
        /// <summary>
        /// Показывает диалоговое окно, используя систему процедурной генерации форм DialogSimpleForm
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public bool ShowDialog(string title)
        {
            DialogSimpleForm ds = new(this, title);
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
