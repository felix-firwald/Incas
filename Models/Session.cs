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
        public int userId { get; set; }
        public DateTime timeStarted { get; set; }
        public DateTime timeFinished { get; set; }
        public string computer { get; set; }
        public bool active { get; set; }
        public Session() 
        {
            tableName = "Sessions";
        }

        public List<Session> GetAllSessions()
        {
            DataTable dt = StartCommandToService()
                .Select()
                .OrderByDESC("slug")
                .Limit(40)
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
            DataTable dt = StartCommandToService()
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
                        {"slug", $"'{slug}'" },
                        { "user", $"'{user}'" },
                        { "userId", $"'{userId}'" },
                        { "timeStarted", $"'{timeStarted}'" },
                        { "computer", $"'{computer}'" },
                        { "active", BoolToInt(active).ToString() },
                    })
                    .ExecuteVoid();
            });
        }
        public void CloseSession()
        {
            StartCommandToService()
                .Update("active", $"0")
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
