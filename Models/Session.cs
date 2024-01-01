using Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Models
{
    public class Session : Model
    {
        public string slug { get; set; }
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
                .WhereEqual("active", "1")
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
        private string GenerateSlug()
        {
            return DateTime.Now.ToString("yyMMddHHmmssffff");
        }
        public void AddSession()
        {
            //if (DoesUserHaveNotClosedSessions())
            //{
            //    throw new UserAlreadyOnlineException();
            //}
            this.slug = GenerateSlug();
            this.user = ProgramState.User;
            this.timeStarted = DateTime.Now;
            this.computer = ProgramState.GetComputerId();
            this.active = true;
            StartCommand()
                .Insert(new Dictionary<string, string>
                {
                    {"slug", $"'{slug}'" },
                    { "user", $"'{user}'" },
                    { "timeStarted", $"'{timeStarted}'" },
                    { "computer", $"'{computer}'" },
                    { "active", BoolToInt(active).ToString() },
                })
                .ExecuteVoid();
        }
        public void CloseSession()
        {
            StartCommand()
                .Update("active", $"0")
                .Update("timeFinished", $"{DateTime.Now}")
                .WhereEqual("slug", this.slug)
                .ExecuteVoid();
        }
        public bool IsSessionActive()
        {
            DataRow dr = StartCommand()
                    .Select()
                    .WhereEqual("slug", this.slug)
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
