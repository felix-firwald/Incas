using Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Models
{
    public struct SSession
    {
        public string slug { get; set; }
        public string user { get; set; }
        public int userId { get; set; }
        public DateTime timeStarted { get; set; }
        public DateTime timeFinished { get; set; }
        public string computer { get; set; }
        public bool active { get; set; }

        public Session AsModel()
        {
            Session session = new();
            session.slug = slug;
            session.user = user;
            session.userId = userId;
            session.timeStarted = timeStarted;
            session.timeFinished = timeFinished;
            session.computer = computer;
            session.active = active;
            return session;
        }
    }
    public class Session : Model
    {
        public string slug { get; set; }
        public string user { get; set; }
        public int userId { get; set; }
        public DateTime timeStarted { get; set; }
        public DateTime timeFinished { get; set; }
        public string computer { get; set; }
        public bool active { get; set; }
        public Session()
        {
            tableName = "Sessions";
        }
        public SSession AsStruct()
        {
            SSession s = new();
            s.slug = slug;
            s.user = user;
            s.userId = userId;
            s.timeStarted = timeStarted;
            s.timeFinished = timeFinished;
            s.computer = computer;
            s.active = active;
            return s;
        }

        public List<SSession> GetAllSessions()
        {
            DataTable dt = StartCommandToService()
                .Select()
                .OrderByDESC("slug")
                .Limit(40)
                .Execute();
            List<SSession> sessions = new();
            foreach (DataRow dr in dt.Rows)
            {
                this.Serialize(dr);
                sessions.Add(this.AsStruct());
            }
            return sessions;
        }
        public List<Session> GetOpenedSessions(bool opened = true)
        {
            DataTable dt = StartCommandToService()
                .Select()
                .WhereEqual("active", opened ? 1 : 0)
                .GroupBy("userId")
                .Having("MAX(slug)")
                .OrderByASC("user")
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

        public async void ClearOldestSessions()
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                StartCommandToService()
                    .Delete()
                    .WhereLess("slug", DateTime.Now.AddDays(-7).ToString("yyMMddHHmmssffff"), false)
                    .ExecuteVoid();
            });
        }

        private string GenerateSlug()
        {
            return DateTime.Now.ToString("yyMMddHHmmssffff");
        }
        public async void AddSession()
        {
            this.slug = GenerateSlug();
            this.user = $"{ProgramState.CurrentUser.fullname}";
            this.userId = ProgramState.CurrentUser.id;
            this.timeStarted = DateTime.Now;
            this.computer = RegistryData.GetComputer();
            this.active = true;
            await System.Threading.Tasks.Task.Run(() =>
            {
                StartCommandToService()
                    .Insert(new Dictionary<string, string>
                    {
                        {"slug", slug },
                        { "user", user },
                        { "userId", userId.ToString() },
                        { "timeStarted", timeStarted.ToString() },
                        { "computer", computer },
                        { "active", BoolToInt(active).ToString() },
                    })
                    .ExecuteVoid();
            });
        }
        public void CloseSession()
        {
            StartCommandToService()
                .Update("active", "0")
                .Update("timeFinished", $"{DateTime.Now}")
                .WhereEqual("slug", this.slug)
                .ExecuteVoid();
        }
        public bool IsSessionActive()
        {
            DataRow dr = StartCommandToService()
                    .Select()
                    .WhereEqual("slug", this.slug)
                    .ExecuteOne();
            this.Serialize(dr);
            if (this.active != true)
            {
                throw new SessionBrokenException();
            }
            return true;
        }
    }
}
