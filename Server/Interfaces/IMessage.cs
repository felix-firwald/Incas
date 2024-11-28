using Incas.Server.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Server.Interfaces
{
    internal interface IMessage
    {
        public Guid MessageId { get; set; } 
        public DirectionMode Direction { get; set; }
        public string Data { get; set; }
    }
}
