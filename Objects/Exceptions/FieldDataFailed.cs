using System;

namespace Incas.Objects.Exceptions
{
    internal class FieldDataFailed : Exception
    {
        public FieldDataFailed(string message) : base(message) { }
    }
}
