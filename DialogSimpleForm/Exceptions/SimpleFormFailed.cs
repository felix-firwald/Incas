using System;

namespace Incas.DialogSimpleForm.Exceptions
{
    public class SimpleFormFailed : Exception
    {
        public SimpleFormFailed(string message) : base(message) { }
    }
}
