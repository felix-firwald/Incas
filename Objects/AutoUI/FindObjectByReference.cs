using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using Incas.Objects.Engine;
using Incas.Objects.Models;
using Incas.Objects.Views.Pages;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для FindObjectByReference.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class FindObjectByReference : AutoUIBase
    {
        protected override string FinishButtonText { get => "Найти объект"; }
        #region Data

        [Description("Ссылка на объект")]
        public string link { get; set; }
        #endregion

        public FindObjectByReference()
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
            try
            {
                ObjectReference or = ObjectReference.Parse(this.link);
                if (or.IsCorrect)
                {
                    ObjectCard card = new();
                    card.MinWidth = 600;
                    card.MaxWidth = 600;
                    Class @class = new(or.Class);
                    card.SetClass(@class);
                    IObject @object = Processor.GetObject(@class, or.Object);
                    card.UpdateFor(@object);
                    DialogsManager.ShowPageWithGroupBox(card, "Поиск: " + @class.Name, "FIND" + this.link, TabType.Usual);
                }
                else
                {
                    DialogsManager.ShowExclamationDialog("Ссылка на объект или класс не установлена.", "Поиск прерван");
                }
            }
            catch
            {
                DialogsManager.ShowErrorDialog("Не удалось расшифровать ссылку на объект, либо ссылка отсылает на несуществующий класс.");
            }
        }
        #endregion
    }
}
