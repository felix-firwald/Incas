using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using IncasEngine.Core;
using System.ComponentModel;

namespace Incas.Testing.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для SendMessageToServer.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class SendMessageToServer : AutoUIBase
    {
        #region Data
        [Description("Сообщение")]
        public string Message { get; set; }
        #endregion

        public SendMessageToServer()
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
            EngineGlobals.Client.Send(this.Message);
        }
        #endregion
    }
}
