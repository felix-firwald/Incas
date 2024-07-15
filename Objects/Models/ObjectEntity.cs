using Incas.Core.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Models
{
    class ObjectEntity : Model
    {
        public Guid Id { get; set; }
        public string VisibleName { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public bool GenerateTab { get; set; }
        public ObjectEntity()
        {
            this.tableName = "Objects";
        }
         
    }
}
