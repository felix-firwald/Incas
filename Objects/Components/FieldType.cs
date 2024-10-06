using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Components
{
    public enum FieldType
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
