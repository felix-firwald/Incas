using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Server.Components
{
    public struct ServerMessage : Interfaces.IMessage
    {
        public Guid MessageId { get; set; }
        public DirectionMode Direction { get; set; }
        public ServerMessageType Type;
        public string Data { get; set; }
    }
}
