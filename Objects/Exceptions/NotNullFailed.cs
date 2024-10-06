using System;

namespace Incas.Objects.Exceptions
{
    internal class NotNullFailed : Exception
    {
        public NotNullFailed(string message) : base(message) { }
    }
}
