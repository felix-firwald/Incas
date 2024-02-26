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
    public struct SUser
    {
        public int id { get; set; }
        public string username { get; set; }
        public string sign { get; set; }
        public string surname { get; set; }
        public string secondName { get; set; }
        public string fullname { get; set; }
        public string post { get; set; }
        public string sector { get; set; }
        public string context { get; set; }

        public User AsModel()
        {
            User user = new();
            user.id = id;
            user.username = username;
            user.sign = sign;
            user.surname = surname;
            user.secondName = secondName;
            user.fullname = fullname;
            user.post = post;
            user.sector = sector;
            user.context = context; 
            return user;
        }
    }
    
    public class User : Model
    {
        public int id { get; set; }
        public string username { get; set; }
        public string sign { get; set; }
        public string surname { get; set; }
        public string secondName { get; set; }
        public string fullname { get; set; }
        public string post { get; set; }
        public string sector { get; set; }
        public string context { get; set; }

        public User() 
        {
            tableName = "Users";
        }
        #region Base
        public List<User> GetAllUsers()
        {
            List<User> output = new List<User>();
            DataTable dt = StartCommandToService()
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
            return StartCommandToService()
                .Select()
                .Execute();
        }
        public User GetUserByName()
        {
            DataRow dr = StartCommandToService()
                    .Select()
                    .WhereEqual("username", this.username)
                    .ExecuteOne();
            this.Serialize(dr);
            return this;
        }

        public void GenerateSign()
        {
            this.sign = ProgramState.GenerateSlug(12);
        }

        public void UpdateUser()
        {
            StartCommandToService()
                .Update("surname", surname)
                .Update("secondName", secondName)
                .Update("fullname", fullname)
                .Update("post", post)
                .Update("sector", sector)
                .WhereEqual("id", id.ToString())
                .ExecuteVoid();
        }
        public void AddUser()
        {
            if (string.IsNullOrEmpty(sign))
            {
                GenerateSign();
            }
            StartCommandToService()
                .Insert(new Dictionary<string, string>
                {
                    { "username", username},
                    { "sign", sign },
                    { "surname", surname },
                    { "secondName", secondName },
                    { "fullname", fullname },
                    { "post", post },
                    { "sector", sector },
                    { "context", context }
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
            DataTable dt = StartCommandToService()
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

        public void RemoveUser()
        {
            StartCommandToService()
                .Delete()
                .WhereEqual("username", username)
                .ExecuteVoid();
        }
        #endregion
        private string GetKey()
        {
            string result = Cryptographer.GenerateKey(this.sign);
            return result;
        }
        public UserParameters GetParametersContext()
        {
            return JsonConvert.DeserializeObject<UserParameters>(Cryptographer.DecryptString(GetKey(), this.context));
            //return UserContextor.GetContext(this);
        }
        public void SaveParametersContext(UserParameters parameters)
        {
            this.context = Cryptographer.EncryptString(GetKey(), JsonConvert.SerializeObject(parameters));
        }
    }
}
