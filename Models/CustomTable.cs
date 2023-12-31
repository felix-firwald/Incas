using Common;
using Incubator_2.Common;
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
        public string view_name;
        public string type;
        public bool not_null;
        public bool is_id;
        public bool is_uniq;
        public string fktable;
        public string fkfield;
    }
    class CustomTable : Model
    {
        public int id { get; set; }
        public string name { get; set; }
        public string viewName { get; set; }
        public List<CustomField> fields { get; set; }

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
                table.fields = DeserializeFields(dr["fields"].ToString());
                tables.Add(table);
            }
            return tables;
        }
        private List<CustomField> DeserializeFields(string str)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<CustomField>>(str);
        }
        private string SerializeFields()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this.fields);
        }
        public CustomTable AddCustomTable()
        {
            StartCommand().Insert(new Dictionary<string, string>
            {
                { "name", $"'{name}'" },
                { "viewName", $"'{viewName}'" },
                { "fields", $"'{SerializeFields()}'" },
            }).ExecuteVoid();
            return this;
        }
        public CustomTable Update()
        {
            StartCommand()
                .Update("viewName", name)
                .Update("fields", SerializeFields())
                .WhereEqual("id", id.ToString())
                .ExecuteVoid();
            return this;
        }
        public CustomTable GetCustomTableByName()
        {
            DataRow dr = StartCommand()
                .Select()
                .WhereEqual("name", name)
                .ExecuteOne();
            Serialize(dr);
            return this;
        }
        public void RemoveCustomTable()
        {
            StartCommand().Delete().WhereEqual("id", id.ToString());
        }
        
    }
}
