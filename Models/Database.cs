using Common;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Incubator_2.Models
{
    public struct SDatabase
    {
        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string sectors { get; set; }
        public Database AsModel()
        {
            Database db = new();
            db.id = id;
            db.name = name;
            db.sectors = sectors;
            db.path = path;
            return db;
        }
    }
    public class Database : Model
    {
        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string sectors { get; set; }

        public Database()
        {
            tableName = "Databases";
        }
        public SDatabase AsStruct()
        {
            SDatabase db = new();
            db.id = id;
            db.name = name;
            db.sectors = sectors;
            db.path = path;
            return db;
        }
        public void AddDatabase()
        {
            path = ProgramState.GenerateSlug(12);
            SQLiteConnection.CreateFile($"{ProgramState.CustomDatabasePath}\\{path}.dbinc");
            StartCommandToService()
                .Insert(new()
                {
                    { nameof(name), name },
                    { nameof(path), path },
                    { nameof(sectors), sectors },
                })
                .ExecuteVoid();

        }
        private List<SDatabase> ParseToList(DataTable dt)
        {
            List<SDatabase> dbs = new();
            foreach (DataRow dr in dt.Rows)
            {
                this.Serialize(dr);
                dbs.Add(this.AsStruct());
            }
            return dbs;
        }
        public List<SDatabase> GetActualDatabases()
        {
            DataTable dt = StartCommandToService()
                .Select()
                .WhereLike(nameof(sectors), ProgramState.CurrentSector.slug)
                .Execute();
            return ParseToList(dt);
        }
        public List<SDatabase> GetAllDatabases()
        {
            DataTable dt = StartCommandToService()
                .Select()
                .Execute();
            return ParseToList(dt);
        }
    }
}
