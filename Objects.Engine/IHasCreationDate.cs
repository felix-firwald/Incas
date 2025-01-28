using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Engine
{
    public interface IHasCreationDate
    {
        public DateTime CreationDate { get; set; }
    }
}
