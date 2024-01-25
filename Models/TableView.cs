using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.Models
{
    public class TableView : Model
    {
        public int id { get; set; }
        public string database { get; set; }
        public string table { get; set; }
        public string name { get; set; }
        public string where { get; set; }
        public string orderby { get; set; }
        public string groupby { get; set; }
        public string joins { get; set; }

        public TableView()
        {
            tableName = "tableViews";
        }
        public void AddView()
        {

        }
        
    }
}
