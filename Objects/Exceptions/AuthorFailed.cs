using System;

namespace Incas.Objects.Exceptions
{
    internal class AuthorFailed : Exception
    {
        public AuthorFailed(string message) : base(message) { }
    }
}
