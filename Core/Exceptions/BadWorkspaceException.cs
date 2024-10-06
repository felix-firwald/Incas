using System;

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
