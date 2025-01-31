using Incas.Objects.Components;
using Incas.Objects.Engine;
using Incas.Objects.Interfaces;
using Incas.Objects.ServiceClasses.Users.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Incas.Objects.ServiceClasses.Groups.Components
{
    public sealed class Group : IObject
    {
        public IClass Class { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<FieldData> Fields { get; set; }

        /// <summary>
        /// Stored in <see cref="Helpers.DataField"/>
        /// </summary>
        public GroupData Data { get; set; }

        public Dictionary<string, string> AddServiceFields(Dictionary<string, string> result)
        {
            result.Add(Helpers.DataField, JsonConvert.SerializeObject(this.Data));
            return result;
        }

        public Group(IClass @class)
        {
            this.Class = @class;
            this.Data = new();
        }

        public IObject Copy()
        {
            Group obj = new(this.Class)
            {
                Fields = this.Fields,
                Data = this.Data
            };
            return obj;
        }

        public void Initialize()
        {

        }
        /// <summary>
        /// Procedural, not for save in objects map
        /// </summary>
        public bool IsWorkspaceSettingsVisible
        {
            get
            {
                return this.Data.CanGroupViewingWorkspaceTab();
            }
        }
        /// <summary>
        /// Procedural, not for save in objects map
        /// </summary>
        public bool IsUsersSettingsVisible
        {
            get
            {
                return this.Data.UserClassPermissions.IsTabVisible();
            }
        }
        /// <summary>
        /// Procedural, not for save in objects map
        /// </summary>
        public bool IsGroupSettingsVisible
        {
            get
            {
                return this.Data.GroupClassPermissions.IsTabVisible();
            }
        }
        
        #region Classes Permissions
        public GroupClassPermissionSettings GetClassPermissions(Guid classId)
        {
            return this.Data.GetClassPermissions(classId);
        }

        public void ParseServiceFields(DataRow dr)
        {
            this.Data = JsonConvert.DeserializeObject<GroupData>(dr[Helpers.DataField].ToString());
        }
        public static List<GroupItem> GetItems(DataTable dt)
        {
            List<GroupItem> items = new();
            foreach (DataRow dr in dt.Rows)
            {
                GroupItem item = new()
                {
                    Id = Guid.Parse(dr[Helpers.IdField].ToString()),
                    Name = dr[Helpers.NameField].ToString()
                };
                items.Add(item);
            }
            return items;
        }
        public GroupItem AsItem()
        {
            GroupItem item = new()
            {
                Id = this.Id,
                Name = this.Name
            };
            return item;
        }
        #endregion
    }
}
