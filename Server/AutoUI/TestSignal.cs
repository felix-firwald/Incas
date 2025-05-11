using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.Server.Components;
using Incas.DialogSimpleForm.Components;
using System.ComponentModel;
using DocumentFormat.OpenXml.Bibliography;

namespace Incas.Server.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для TestSignal.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class TestSignal : StaticAutoUIBase
    {
        #region Data
        [Description("IP адрес")]
        public string Ip { get; set; }
        [Description("Целевая команда")]
        public Selector Target { get; set; }
        #endregion

        public TestSignal()
        {
            this.Ip = ProgramState.GetLocalIPAddress().ToString();
            this.Target = new(new());
            this.Target.Pairs.Add(ClientMessageType.InitialRequest, "InitialRequest");
            this.Target.Pairs.Add(ClientMessageType.Authorization, "Authorization");
            this.Target.Pairs.Add(ClientMessageType.ServiceAction, "ServiceAction");
            this.Target.Pairs.Add(ClientMessageType.ObjectAction, "ObjectAction");
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
            Client.ConnectToService();
            ClientMessage cm = new()
            {
                Direction = DirectionMode.Request,
                Type = (ClientMessageType)this.Target.SelectedObject
            };
            Client.IpAdress = System.Net.IPAddress.Parse(this.Ip);
            Client.Write(cm);
        }
        #endregion
    }
}
