using Incas.Core.Classes;
using Incas.Core.Views.Controls;
using Incas.DialogSimpleForm.Components;
using Incas.Objects.Views.Pages;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
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
                    card.OnFilterRequested += this.Card_OnFilterRequested;
                    Class @class = new(or.Class);
                    this.classSource = @class;
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
        private IClass classSource { get; set; }
        private void Card_OnFilterRequested(IncasEngine.ObjectiveEngine.Common.FieldData data)
        {
            ObjectsList list = new(this.classSource);
            list.OpenInNewTabButton.Visibility = System.Windows.Visibility.Collapsed;
            DialogsManager.ShowPageWithGroupBox(list, this.classSource.GetClassData().ListName, MainWindowButtonTab.ClassPrefix + this.classSource.Id, TabType.Usual);
            list.UpdateViewWithFilter(data);
        }
        #endregion
    }
}
