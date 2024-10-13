using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Interfaces
{
    public interface ISimpleFiller : IFillerBase
    {
        public string GetValue();
        public string GetTagName();
        public Dictionary<string, string> GetDataFromObjectRelation();
    }
}
