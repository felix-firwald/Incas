using Incas.Core.Classes;
using System;
using System.Collections.Generic;
using System.Data;

namespace Incas.Users.Models
{
    public struct SSession
    {
        public string slug { get; set; }
        public string user { get; set; }
        public Guid userId { get; set; }
        public DateTime timeStarted { get; set; }
        public DateTime timeFinished { get; set; }
        public string computer { get; set; }
        public bool active { get; set; }

        public Session AsModel()
        {
            Session session = new()
            {
                slug = this.slug,
                user = this.user,
                userId = this.userId,
                timeStarted = this.timeStarted,
                timeFinished = this.timeFinished,
                computer = this.computer,
                active = this.active
            };
            return session;
        }
    }
    public class Session : Model
    {
        public string slug { get; set; }
        public string user { get; set; }
        public Guid userId { get; set; }
        public DateTime timeStarted { get; set; }
        public DateTime timeFinished { get; set; }
        public string computer { get; set; }
        public bool active { get; set; }
        public Session()
        {
            this.tableName = "Sessions";
        }
        public SSession AsStruct()
        {
            SSession s = new()
            {
                slug = this.slug,
                user = this.user,
                userId = this.userId,
                timeStarted = this.timeStarted,
                timeFinished = this.timeFinished,
                computer = this.computer,
                active = this.active
            };
            return s;
        }

        public List<SSession> GetAllSessions()
        {
            DataTable dt = this.StartCommandToService()
                .Select()
                .OrderByDESC("slug")
                .Limit(40)
                .Execute();
            List<SSession> sessions = [];
            foreach (DataRow dr in dt.Rows)
            {
                this.Serialize(dr);
                sessions.Add(this.AsStruct());
            }
            return sessions;
        }
        public List<Session> GetOpenedSessions(bool opened = true)
        {
            DataTable dt = this.StartCommandToService()
                .Select()
                .WhereEqual("active", opened ? 1 : 0)
                .GroupBy("userId")
                .Having("MAX(slug)")
                .OrderByASC("user")
                .Execute();
            List<Session> sessions = [];
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
                this.StartCommandToService()
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
            this.slug = this.GenerateSlug();
            this.user = $"{ProgramState.CurrentUser.fullname}";
            this.userId = ProgramState.CurrentUser.id;
            this.timeStarted = DateTime.Now;
            this.active = true;
            await System.Threading.Tasks.Task.Run(() =>
            {
                this.StartCommandToService()
                    .Insert(new Dictionary<string, string>
                    {
                        {"slug", this.slug },
                        { "user", this.user },
                        { "userId", this.userId.ToString() },
                        { "timeStarted", this.timeStarted.ToString() },
                        { "active", this.BoolToInt(this.active).ToString() },
                    })
                    .ExecuteVoid();
            });
        }
        public void CloseSession()
        {
            this.StartCommandToService()
                .Update("active", "0")
                .Update("timeFinished", $"{DateTime.Now}")
                .WhereEqual("slug", this.slug)
                .ExecuteVoid();
        }
        public bool IsSessionActive()
        {
            DataRow dr = this.StartCommandToService()
                    .Select()
                    .WhereEqual("slug", this.slug)
                    .ExecuteOne();
            this.Serialize(dr);
            return this.active != true ? throw new SessionBrokenException() : true;
        }
    }
}
