using Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Models
{
    public class Session : Model
    {
        public int id { get; set; }
        public string user { get; set; }
        public DateTime timeStarted { get; set; }
        public DateTime timeFinished { get; set; }
        public string computer { get; set; }
        public bool active { get; set; }
        public Session() 
        {
            tableName = "Sessions";
        }

        public bool DoesUserHaveNotClosedSessions()
        {
            //StartCommand().Count()
            //.WhereEqual("user", user)
            //.WhereEqual("active", "true")
            //if (ParseCount(Execute(DatabasePermissions.All)) != 0)
            //{
            //    return true;
            //}
            return false;
        }
        public List<Session> GetAllSessions()
        {
            DataTable dt = StartCommand()
                .Select()
                .Execute();
            List<Session> sessions = new List<Session>();
            foreach (DataRow dr in dt.Rows)
            {
                Session s = new Session();
                s.Serialize(dr);
                sessions.Add(s);
            }
            return sessions;
        }
        public List<Session> GetOpenedSessions()
        {
            DataTable dt = StartCommand()
                .Select()
                .WhereEqual("active", "True")
                .Execute();
            List<Session> sessions = new List<Session>();
            foreach (DataRow dr in dt.Rows)
            {
                Session s = new Session();
                s.Serialize(dr);
                sessions.Add(s);
            }
            return sessions;
        }
        public int GetIdOfSession()
        {
            return int.Parse(
                    StartCommand()
                        .Select("id")
                        .WhereEqual("timeStarted", this.timeStarted.ToString())
                        .ExecuteOne()["id"].ToString()
            );
            // return int.Parse(GetOne(Execute())["id"].ToString());
        }
        public void AddSession()
        {
            if (DoesUserHaveNotClosedSessions())
            {
                throw new UserAlreadyOnlineException();
            }
            this.user = ProgramState.User;
            this.timeStarted = DateTime.Now;
            this.computer = ProgramState.GetComputerId();
            this.active = true;
            StartCommand()
                .Insert(new Dictionary<string, string>
                {
                    { "user", $"'{user}'" },
                    { "timeStarted", $"'{timeStarted}'" },
                    { "computer", $"'{computer}'" },
                    { "active", $"'{active}'" },
                })
                .ExecuteVoid();
            this.id = GetIdOfSession();
            ProgramState.CurrentSession = this;
        }
        public void CloseSession()
        {
            StartCommand()
                .Update("active", $"{false}")
                .Update("timeFinished", $"{DateTime.Now}")
                .WhereEqual("id", this.id.ToString(), false)
                .ExecuteVoid();
        }
        public bool IsSessionActive()
        {
            DataRow dr = StartCommand()
                    .Select()
                    .WhereEqual("id", this.id.ToString(), false)
                    .ExecuteOne();
            Clear();
            this.Serialize(dr);
            if (this.active != true)
            {
                throw new SessionBrokenException();
            }
            return true;
        }
    }
}
