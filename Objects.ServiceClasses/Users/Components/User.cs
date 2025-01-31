using Incas.Core.Classes;
using Incas.Objects.Components;
using Incas.Objects.Engine;
using Incas.Objects.Interfaces;
using Incas.Objects.ServiceClasses.Groups.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Incas.Objects.ServiceClasses.Users.Components
{
    public sealed class User : IObject
    {
        public IClass Class { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<FieldData> Fields { get; set; }

        /// <summary>
        /// Stored in <see cref="Helpers.PasswordField"/>
        /// </summary>
        public UserData Data { get; set; }

        /// <summary>
        /// Stored in <see cref="Helpers.GroupField"/>
        /// </summary>
        public Guid Group { get; set; }

        public User(IClass @class)
        {
            this.Class = @class;
            this.Data = new();
        }

        public IObject Copy()
        {
            User user = new(this.Class)
            {
                Fields = this.Fields,
                Group = this.Group
            };
            return user;
        }

        public Dictionary<string, string> AddServiceFields(Dictionary<string, string> result)
        {
            string data = JsonConvert.SerializeObject(this.Data);
            data = FieldCryptographer.EncryptFieldValue(ProgramState.CurrentWorkspace.GetDefinition().Id, this.Class.Id, this.Id, data);
            result.Add(Helpers.DataField, data);
            result.Add(Helpers.GroupField, this.Group.ToString());
            return result;
        }

        public void Initialize()
        {
            
        }

        public void ParseServiceFields(DataRow dr)
        {
            string data = dr[Helpers.DataField].ToString();
            data = FieldCryptographer.DecryptFieldValue(ProgramState.CurrentWorkspace.GetDefinition().Id, this.Class.Id, this.Id, data);
            this.Data = JsonConvert.DeserializeObject<UserData>(data);
            this.Group = Guid.Parse(dr[Helpers.GroupField].ToString());
        }
        public static List<UserItem> GetItems(DataTable dt)
        {
            List<UserItem> items = new();
            foreach (DataRow dr in dt.Rows)
            {
                UserItem item = new()
                {
                    Id = Guid.Parse(dr[Helpers.IdField].ToString()),
                    Name = dr[Helpers.NameField].ToString()
                };
                items.Add(item);
            }
            return items;
        }
        public bool IsRightPassword(string password)
        {
            return this.Data.Password == password;
        }
        public UserItem AsItem()
        {
            UserItem ui = new();
            ui.Id = this.Id;
            ui.Name = this.Name;
            return ui;
        }
        public Group GetGroup()
        {
            return (Group)Processor.GetObject(ProgramState.CurrentWorkspace.GetDefinition().ServiceGroups, this.Group);
        }
    }
}
