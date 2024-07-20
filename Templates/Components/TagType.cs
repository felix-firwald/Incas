using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Templates.Components
{
    public enum TagType
    {
        Variable,
        Text,
        Number,
        Relation,
        LocalEnumeration,
        GlobalEnumeration,
        LocalConstant,
        GlobalConstant,
        HiddenField,
        Date,
        Generator,
        Table,
        Macrogenerator
    }
}
