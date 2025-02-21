using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using IncasEngine.ClientServer.Core;
using IncasEngine.Core;
using System.ComponentModel;

namespace Incas.Testing.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для ConnectToServer.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class ConnectToServer : AutoUIBase
    {
        #region Data
        [Description("Строка подключения")]
        [CanBeNull]
        public string ConnectionString { get; set; }

        [Description("IP")]
        public string IP { get; set; }

        [Description("Порт")]
        public int Port { get; set; }
        #endregion

        public ConnectToServer()
        {
            this.ConnectionString = $"ws://{EngineGlobals.CurrentWorkspace.GetDefinition().Id}.incas/simple_message";
            
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
            if (string.IsNullOrEmpty(this.ConnectionString))
            {
                SocketClient client = new($"ws://localhost:{this.Port}/simple_message");
                EngineGlobals.Client = client;
            }
            else
            {
                SocketClient client = new(this.ConnectionString);
                EngineGlobals.Client = client;
            }
            
        }
        #endregion
    }
}
