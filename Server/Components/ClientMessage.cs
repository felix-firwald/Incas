using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Server.Components
{
    public struct ClientMessage : Interfaces.IMessage
    {
        public Guid MessageId { get; set; }
        public Guid Session;
        public DirectionMode Direction { get; set; }
        public ClientMessageType Type;
        public string Data { get; set; }
    }
}
