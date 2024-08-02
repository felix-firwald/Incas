using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Core.Exceptions
{
    public class BadWorkspaceException : Exception
    {
        public BadWorkspaceException(string message) : base(message)
        {

        }
        public BadWorkspaceException(string message, Exception innerException)
        : base(message, innerException)
        {

        }
    }
}
