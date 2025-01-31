using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Engine
{
    public class ServiceFieldAttribute : Attribute
    {
        public string Name { get; private set; }
        public ServiceFieldAttribute(string name)
        {
            this.Name = name;
        }
    }
}
