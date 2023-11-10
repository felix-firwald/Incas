using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace Models
{
    public class User : Model
    {
        public int id { get; set; }
        public string username { get; set; }
        public string surname { get; set; }
        public string fullname { get; set; }
        public string post { get; set; }
        public string password { get; set; }
        public PermissionGroup status { get; set; }

        public User() 
        {
            tableName = "Users";
            definition = "id INTEGER PRIMARY KEY AUTOINCREMENT,\n" +
                "username STRING UNIQUE ON CONFLICT ROLLBACK NOT NULL,\n" +
                "fullname STRING,\n" +
                "surname STRING,\n" +
                "post INTEGER REFERENCES Posts (id),\n" +
                "password STRING UNIQUE ON CONFLICT ROLLBACK NOT NULL,\n" +
                "status STRING NOT NULL\n";
        }
        public List<User> GetAllUsers()
        {
            List<User> output = new List<User>();
            DataTable dt = StartCommand()
                .Select()
                .Execute();
            foreach (DataRow dr in dt.Rows)
            {
                User mu = new User();
                mu.Serialize(dr);
                mu.status = (PermissionGroup)Enum.Parse(typeof(PermissionGroup), dr["status"].ToString());
                output.Add(mu);
            }
            return output;
        }
        public DataTable GetAllUsersAsDT()
        {
            return StartCommand()
                .Select()
                .Execute();
        }
        public User GetUserByName()
        {
            DataRow dr = GetOne(
                StartCommand()
                    .Select()
                    .WhereEqual("username", username)
                    .Execute()
            );
            this.Serialize(dr);
            this.status = (PermissionGroup)Enum.Parse(typeof(PermissionGroup), dr["status"].ToString());
            return this;
        }
        public void UpdatePassword()
        {
            StartCommand()
                .Update("password", password)
                .WhereEqual("username", username)
                .ExecuteVoid();
        }
        public void UpdateAll()
        {
            StartCommand()
                .Update("password", password)
                .Update("surname", surname)
                .Update("username", username)
                .Update("fullname", fullname)
                .Update("post", post)
                .Update("status", status.ToString())
                .WhereEqual("username", username)
                .ExecuteVoid();
        }
        public void AddUser()
        {
            StartCommand()
                .Insert(new Dictionary<string, string>
                {
                    { "username", $"'{username}'" },
                    { "surname", $"'{surname}'" },
                    { "fullname", $"'{fullname}'" },
                    { "password", $"'{password}'" },
                    { "status", $"'{status}'" },
                })
                .ExecuteVoid();
        }
        public bool DoesUserExists(string username)
        {
            DataTable dt = StartCommand()
                .Select("username")
                .WhereEqual("username", username)
                .Execute();
            if (dt != null)
            {
                if (dt.Rows.Count == 0)
                {
                    return false;
                }
                return true;
            }
            return false;
        }
        public bool IsPasswordExists(string pwd)
        {
            DataTable dt = StartCommand()
                .Select()
                .WhereEqual("password", pwd)
                .Execute();
            if (dt.Rows.Count == 0)
            {
                return false;
            }
            DataRow dr = GetOne(dt);
            this.Serialize(dr);
            this.status = (PermissionGroup)Enum.Parse(typeof(PermissionGroup), dr["status"].ToString());
            ProgramState.User = this.username;
            return true;
        }
        public void RemoveUser()
        {
            StartCommand()
                .Delete()
                .WhereEqual("username", username)
                .ExecuteVoid();
        }
    }
}
