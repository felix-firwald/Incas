using Incas.Core.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Incas.Users.Models
{
    public struct GroupData
    {
        public bool IsSuper { get; set; }
    }
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
        private void Update()
        {
            this.StartCommand()
                .Update(new()
                    {
                        { nameof(this.Name), this.Name.ToString() },
                        { nameof(this.Data), this.Data.ToString() }
                    })
                .WhereEqual(nameof(this.Id), this.Id.ToString())
                .ExecuteVoid();
        }
        public Guid Save()
        {
            if (this.Id != Guid.Empty)
            {
                this.Update();
            }
            else
            {
                this.Id = Guid.NewGuid();
                this.StartCommand()
                    .Insert(new()
                        {
                            { nameof(this.Id), this.Id.ToString() },
                            { nameof(this.Name), this.Name.ToString() },
                            { nameof(this.Data), this.Data.ToString() }
                        })
                    .ExecuteVoid();
            }
            return this.Id;
        }
        public void SetData(GroupData settings)
        {
            this.Data = JsonConvert.SerializeObject(settings);
        }
        public GroupData GetData()
        {
            return JsonConvert.DeserializeObject<GroupData>(this.Data);
        }
    }
}
