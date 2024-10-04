using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Core.Exceptions
{
    public class SimpleFormFailed : Exception
    {
        public SimpleFormFailed(string message) : base(message) { }
    }
}
