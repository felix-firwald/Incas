using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Models
{
    public class Post : Model
    {
        public int id { get; set; }
        public string name { get; set; }
        public PermissionGroup permission { get; set; }
        public string rights { get; set; }

        public Post()
        {
            tableName = "Posts";
            definition = "id INTEGER PRIMARY KEY AUTOINCREMENT,\n" +
                "name STRING UNIQUE ON CONFLICT ROLLBACK NOT NULL DEFAULT [Администратор инкубатора],\n" +
                "permission STRING NOT NULL,\n" +
                "rights STRING NOT NULL";
        }
        #region GET
        public List<Post> GetAll()
        {
            DataTable dt = StartCommand()
                                .Select()
                                .Execute();
            List<Post> posts = new List<Post>();
            foreach (DataRow dr in dt.Rows)
            {
                Post mp = new Post();
                mp.Serialize(dr);
                mp.permission = (PermissionGroup)Enum.Parse(typeof(PermissionGroup), dr["permission"].ToString());
                posts.Add(mp);
            }
            return posts;
        }
        public Post GetById()
        {
            DataRow dr = StartCommand()
                            .Select()
                            .WhereEqual("id", id.ToString())
                            .Execute().Rows[0];
            this.Serialize(dr);
            this.permission = (PermissionGroup)Enum.Parse(typeof(PermissionGroup), dr["permission"].ToString());
            return this;
        }
        public List<Post> GetByPermissionGroup(PermissionGroup group)
        {
            DataTable dt = StartCommand()
                                .Select()
                                .WhereEqual("permission", group.ToString())
                                .Execute();
            List<Post> posts = new List<Post>();
            foreach (DataRow dr in dt.Rows)
            {
                Post mp = new Post();
                mp.Serialize(dr);
                mp.permission = group;
                posts.Add(mp);
            }
            return posts;
        }
        #endregion

        #region Create
        public void AddPost()
        {
            StartCommand()
                .Insert(new Dictionary<string, string>
                {
                    { "name", $"'{name}'" },
                    { "permission", $"'{permission}'" },
                    { "rights", $"'{rights}'" },
                })
                .ExecuteVoid();
        }
        #endregion

        #region Update
        public void UpdatePost()
        {
            StartCommand()
                .Update("name", name)
                .Update("permission", permission.ToString())
                .Update("rights", rights)
                .WhereEqual("id", id.ToString())
                .ExecuteVoid();
        }
        #endregion

        #region Delete
        public void DeletePost()
        {
            StartCommand()
                .Delete()
                .WhereEqual("name", name)
                .ExecuteVoid();
        }
        #endregion
    }
}
