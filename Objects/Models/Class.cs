using Incas.Core.Classes;
using Incas.Templates.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Incas.Objects.Models
{
    class ClassData
    {
        List<Field> fields { get; set; }

    }

    class Class : Model
    {
        public Guid identifier { get; set; }
        public string category { get; set; }
        public string visibleName { get; set; }
        public string name { get; set; }
        public ClassData data { get; set; }
        public Class()
        {
            this.tableName = "Classes";
        }
        private List<Class> FromDataTable(DataTable dt)
        {
            List<Class> resulting = new();
            foreach (DataRow dr in dt.Rows)
            {
                this.Serialize(dr);
                this.data = JsonConvert.DeserializeObject<ClassData>(dr["data"].ToString());
                resulting.Add(this);
            }
            return resulting;
        }
        public List<Class> GetAllClasses()
        {
            DataTable dt = this.StartCommandToService().Select().Execute();
            return this.FromDataTable(dt);
        }
        
    }
}
