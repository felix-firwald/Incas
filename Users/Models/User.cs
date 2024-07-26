using Incas.Core.Classes;
using IncasEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;

namespace Incas.Users.Models
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
            User user = new()
            {
                id = this.id,
                username = this.username,
                sign = this.sign,
                surname = this.surname,
                secondName = this.secondName,
                fullname = this.fullname,
                post = this.post,
                context = this.context
            };
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
        public string context { get; set; }

        public User()
        {
            this.tableName = "Users";
        }
        #region Base
        public List<User> GetAllUsers()
        {
            List<User> output = [];
            DataTable dt = this.StartCommandToService()
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
            return this.StartCommandToService()
                .Select()
                .Execute();
        }
        public User GetUserByName()
        {
            DataRow dr = this.StartCommandToService()
                    .Select()
                    .WhereEqual("username", this.username)
                    .ExecuteOne();
            this.Serialize(dr);
            return this;
        }

        public void GenerateSign()
        {
            if (string.IsNullOrEmpty(this.sign))
            {
                this.sign = ProgramState.GenerateSlug(12);
            }
        }
        public void UpdateUser()
        {
            this.StartCommandToService()
                .Update("sign", this.sign)
                .Update("surname", this.surname)
                .Update("secondName", this.secondName)
                .Update("fullname", this.fullname)
                .Update("post", this.post)
                .Update("context", this.context)
                .WhereEqual("id", this.id.ToString())
                .ExecuteVoid();
        }
        public void AddUser()
        {
            if (string.IsNullOrEmpty(this.sign))
            {
                this.GenerateSign();
            }
            this.StartCommandToService()
                .Insert(new Dictionary<string, string>
                {
                    { "username", this.username},
                    { "sign", this.sign },
                    { "surname", this.surname },
                    { "secondName", this.secondName },
                    { "fullname", this.fullname },
                    { "post", this.post },
                    { "context", this.context }
                })
                .ExecuteVoid();
            this.GetUserByName();
        }
        public void SaveUser()
        {
            this.fullname = this.surname + " " + this.secondName;
            if (this.id == 0)
            {
                this.AddUser();
            }
            else
            {
                this.UpdateUser();
            }
        }
        public bool DoesUserExists(string username)
        {
            DataTable dt = this.StartCommandToService()
                .Select("username")
                .WhereEqual("username", username)
                .Execute();
            return dt != null && dt.Rows.Count != 0;
        }

        public void RemoveUser()
        {
            this.StartCommandToService()
                .Delete()
                .WhereEqual("username", this.username)
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
            return JsonConvert.DeserializeObject<UserParameters>(Cryptographer.DecryptString(this.GetKey(), this.context));
        }
        public void SaveParametersContext(UserParameters parameters)
        {
            this.context = Cryptographer.EncryptString(this.GetKey(), JsonConvert.SerializeObject(parameters));
        }
    }
}
