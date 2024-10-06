using System;

namespace Incas.Core.Exceptions
{
    public class SimpleFormFailed : Exception
    {
        public SimpleFormFailed(string message) : base(message) { }
    }
}
