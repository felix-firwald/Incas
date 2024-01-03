using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
            DataRow dr = StartCommand()
                    .Select()
                    .WhereEqual("username", this.username)
                    .ExecuteOne();
            this.Serialize(dr);
            this.status = (PermissionGroup)Enum.Parse(typeof(PermissionGroup), dr["status"].ToString());
            return this;
        }
        public User GetUserByPassword()
        {
            DataRow dr = StartCommand()
                    .Select()
                    .WhereEqual("password", this.password)
                    .ExecuteOne();
                
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
        public void UpdateUser()
        {
            StartCommand()
                .Update("password", password)
                .Update("surname", surname)
                .Update("username", username)
                .Update("fullname", fullname)
                .Update("post", post)
                .Update("status", status.ToString())
                .WhereEqual("id", id.ToString())
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
                    { "post", $"'{post}'" }
                })
                .ExecuteVoid();
            GetUserByName();
        }
        public void SaveUser()
        {
            if (id == 0)
            {
                AddUser();
            }
            else
            {
                UpdateUser();
            }
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
            DataRow dr = dt.Rows[0];
            this.Serialize(dr);
            this.status = (PermissionGroup)Enum.Parse(typeof(PermissionGroup), dr["status"].ToString());
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
