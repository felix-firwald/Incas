using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using Incas.Scripting.Interfaces;
using System.ComponentModel;

namespace Incas.Scripting.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для ScriptHelperCreateForm.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class ScriptHelperCreateForm : StaticAutoUIBase, IScriptHelper
    {
        protected override string FinishButtonText { get => "Вставить код"; }
        #region Data
        [Description("Название формы")]
        public string FormName { get; set; }

        [Description("Название кнопки сохранения")]
        public string FinishName { get; set; }

        [Description("Название кнопки отмены")]
        public string CancelName { get; set; }
        #endregion

        public ScriptHelperCreateForm()
        {
            this.FinishName = "Сохранить";
            this.CancelName = "Отменить";
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

        public string GetScript()
        {
            string result = $"form = AutoUI.Init('{this.FormName}')\n" +
                $"form.FinishButtonText = '{this.FinishName}'\n" +
                $"form.CancelButtonText = '{this.CancelName}'\n" +
                $"form.AddStringField('ключ1', 'Поле 1', '<нет описания>') # добавить на форму строковое поле\n" +
                $"form.AddTextField('ключ2', 'Поле 2', '<нет описания>') # добавить на форму текстовое поле\n" +
                $"form.AddIntField('ключ3', 'Поле 3', '<нет описания>') # добавить на форму поле с числовым заполнением\n" +
                $"form.AddBooleanField('ключ4', 'Поле 4', '<нет описания>') # добавить на форму флажок\n" +
                $"form.AddEnumerationField('ключ5', 'Поле 5', ['вариант 1', 'вариант 2'], '<нет описания>') # добавить на форму выпадающий список\n" +
                $"form.AddDateTimeField('ключ6', 'Поле 6', '<нет описания>') # добавить на форму поле выбора даты\n" +
                $"incas.show_dialog(form)\n" +
                $"if form.Result == True: # если форма заполнена\n\tpass\nelse: # если форма НЕ заполнена или закрыта\n\tpass";
            
            return result;
        }
        #endregion
    }
}
