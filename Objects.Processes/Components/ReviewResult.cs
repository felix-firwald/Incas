using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Processes.Components
{
    public enum ReviewResult
    {
        Undefined, // not stated
        Agreed, // ok
        AgreedWithComment, // ok + comment
        Rejected // not passed
    }
}
