using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Interfaces
{
    public interface ITableFiller : IFillerBase
    {
        public DataTable GetValue();
    }
}
