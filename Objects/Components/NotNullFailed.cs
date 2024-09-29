using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Components
{
    class NotNullFailed : Exception
    {
        public NotNullFailed(string message) : base(message) { }
    }
}
