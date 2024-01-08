using Common;
using Incubator_2.Common;
using Newtonsoft.Json;
using Spire.Doc.Documents.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Models
{
    
    public class User : Model
    {
        public int id { get; set; }
        public string username { get; set; }
        public string sign { get; set; }
        public string surname { get; set; }
        public string secondName { get; set; }
        public string fullname { get; set; }
        public string post { get; set; }

        public User() 
        {
            tableName = "Users";
        }
        #region Base
        public List<User> GetAllUsers()
        {
            List<User> output = new List<User>();
            DataTable dt = StartCommand()
                .Select()
                .OrderByASC("fullname")
                .Execute();
            foreach (DataRow dr in dt.Rows)
            {
                User mu = new User();
                mu.Serialize(dr);
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
            return this;
        }
        //public User GetUserByPassword()
        //{
        //    DataRow dr = StartCommand()
        //            .Select()
        //            .WhereEqual("password", this.password)
        //            .ExecuteOne();
                
        //    this.Serialize(dr);
        //    return this;
        //}
        public void UpdateUser()
        {
            StartCommand()
                .Update("surname", surname)
                .Update("secondName", secondName)
                .Update("fullname", fullname)
                .Update("post", post)
                .WhereEqual("id", id.ToString())
                .ExecuteVoid();
        }
        public void AddUser()
        {
            this.sign = ProgramState.GenerateSlug(12);
            StartCommand()
                .Insert(new Dictionary<string, string>
                {
                    { "username", $"'{username}'" },
                    { "sign", $"'{sign}'" },
                    { "surname", $"'{surname}'" },
                    { "secondName", $"'{secondName}'" },
                    { "fullname", $"'{fullname}'" },
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
        //public bool IsPasswordExists(string pwd)
        //{
        //    DataTable dt = StartCommand()
        //        .Select()
        //        .WhereEqual("password", pwd)
        //        .Execute();
        //    if (dt.Rows.Count == 0)
        //    {
        //        return false;
        //    }
        //    DataRow dr = dt.Rows[0];
        //    this.Serialize(dr);
        //    return true;
        //}
        public void RemoveUser()
        {
            StartCommand()
                .Delete()
                .WhereEqual("username", username)
                .ExecuteVoid();
        }
        #endregion

        public UserParameters GetParametersContext()
        {
            return UserContextor.GetContext(this);
        }
    }
}
