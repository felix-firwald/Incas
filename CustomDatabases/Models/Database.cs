using Incas.Core.Classes;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Incas.CustomDatabases.Models
{
    public class Database : Model
    {
        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }

        public Database()
        {
            this.tableName = "Databases";
        }
        public SDatabase AsStruct()
        {
            SDatabase db = new()
            {
                id = this.id,
                name = this.name,
                path = this.path
            };
            return db;
        }
        public void AddDatabase()
        {
            this.path = ProgramState.GenerateSlug(12);
            SQLiteConnection.CreateFile($"{ProgramState.CustomDatabasePath}\\{this.path}.dbinc");
            this.StartCommandToService()
                .Insert(new()
                {
                    { nameof(this.name), this.name },
                    { nameof(this.path), this.path },
                })
                .ExecuteVoid();

        }
        private List<SDatabase> ParseToList(DataTable dt)
        {
            List<SDatabase> dbs = [];
            foreach (DataRow dr in dt.Rows)
            {
                this.Serialize(dr);
                dbs.Add(this.AsStruct());
            }
            return dbs;
        }
        public List<SDatabase> GetActualDatabases()
        {
            DataTable dt = this.StartCommandToService()
                .Select()
                .Execute();
            return this.ParseToList(dt);
        }
        public List<SDatabase> GetAllDatabases()
        {
            DataTable dt = this.StartCommandToService()
                .Select()
                .Execute();
            return this.ParseToList(dt);
        }
    }
}
