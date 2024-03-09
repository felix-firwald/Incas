using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.Models.Auxiliary
{
    public struct Transfer
    {
        public string name;
        public Dictionary<int, int> content;
    }


    public struct Trigger
    {
        public string name;
    }

    public struct TemplateSettings
    {
        public List<Transfer> transfers;
        public List<Trigger> triggers;
    }
}
