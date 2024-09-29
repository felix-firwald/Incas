using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Exceptions
{
    class FieldDataFailed : Exception
    {
        public FieldDataFailed(string message) : base(message) { }
    }
}
