using Incas.Core.Attributes;
using Incas.DialogSimpleForm.Components;
using System.ComponentModel;

namespace Incas.Testing.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для RunServer.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class RunServer : AutoUIBase
    {
        #region Data
        [Description("Порт")]
        public int Port { get; set; }
        [Description("Полный путь")]
        [CanBeNull]
        public string ConnectionString { get; set; }
        #endregion

        public RunServer()
        {
            this.ConnectionString = "ws://0.0.0.0:9000";
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
            //if (string.IsNullOrEmpty(this.ConnectionString))
            //{
            //    SocketServer server = new(this.Port);
            //}
            //else
            //{
            //    SocketServer server = new(this.ConnectionString);
            //}
        }
        #endregion
    }
}
