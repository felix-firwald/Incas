using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Server.Components
{
    public static class CommonClientServerLogic
    {
        public const char EndMessage = '¦';

        public static byte[] GetBytesWithSep(string message)
        {
            return Encoding.UTF8.GetBytes(message + EndMessage);
        }
    }
}
