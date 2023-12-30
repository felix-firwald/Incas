using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.Models
{
    struct CustomField
    {
        public string name;
        public string viewName;
        
    }
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
        
        public List<CustomTable> GetAllTables()
        {
            DataTable dt = StartCommand()
                .Select()
                .Execute();
            List<CustomTable> tables = new List<CustomTable>();
            foreach (DataRow dr in dt.Rows)
            {
                CustomTable table = new CustomTable();
                table.Serialize(dr);
                tables.Add(table);
            }
            return tables;
        }

    }
}
