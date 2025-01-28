using Incas.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Components
{
    public class ObjectComment
    {       
        public Guid Id { get; set; }
        public Guid Class { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid AuthorId { get; set; }
        public CommentType Type { get; set; }
        public Guid TargetObject { get; set; }
        public string Data { get; set; }
    }
}
