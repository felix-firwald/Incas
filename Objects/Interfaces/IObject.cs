using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Interfaces
{
    interface IObject
    {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid AuthorId { get; set; }
    }
}
