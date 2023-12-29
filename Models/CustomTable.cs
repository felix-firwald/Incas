using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.Models
{
    class CustomTable : Model
    {
        public int id { get; set; }
        public string name { get; set; }
        public string viewName { get; set; }
        public bool fields { get; set; }

        public CustomTable()
        {
            tableName = "CustomTables";
        }


    }
}
