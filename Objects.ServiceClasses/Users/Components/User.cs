using Incas.Objects.Components;
using Incas.Objects.Engine;
using Incas.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Incas.Objects.ServiceClasses.Users.Components
{
    public sealed class User : IObject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<FieldData> Fields { get; set; }

        /// <summary>
        /// Stored in <see cref="Helpers.PasswordField"/>
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Stored in <see cref="Helpers.GroupField"/>
        /// </summary>
        public Guid Group { get; set; }

        public IObject Copy()
        {
            User user = new()
            {
                Fields = this.Fields,
                Group = this.Group
            };
            return user;
        }

        public Dictionary<string, string> AddServiceFields(Dictionary<string, string> result)
        {
            result.Add(Helpers.PasswordField, this.Password);
            result.Add(Helpers.GroupField, this.Group.ToString());
            return result;
        }

        public void Initialize()
        {
            
        }

        public void ParseServiceFields(DataRow dr)
        {
            
        }
    }
}
