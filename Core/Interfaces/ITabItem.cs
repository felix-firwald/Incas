using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Core.Interfaces
{
    public interface ITabItem
    {
        public delegate void TabAction(ITabItem item);
        public event TabAction OnClose;
        public string Id { get; set; }
    }
}
