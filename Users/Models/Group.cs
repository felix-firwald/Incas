using Incas.Core.Classes;
using System;
using System.Collections.Generic;
using System.Data;

namespace Incas.Users.Models
{
    public class Group : Model
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }
        public Group()
        {
            this.tableName = "Groups";
        }
        private Group GetFromDataRow(DataRow dr)
        {
            this.Serialize(dr);
            return this;
        }
        private List<Group> GetFromDataTable(DataTable dt)
        {
            List<Group> result = new();
            foreach (DataRow dr in dt.Rows)
            {
                Group group = new();
                group.Serialize(dr);
                result.Add(group);
            }
            return result;
        }
    }
}
