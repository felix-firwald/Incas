using Common;
using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.Models
{
    
    public enum CommandType
    {
        Read,
        Update
    }
    public struct ReadCommand
    {
        public string Select;
        public string From;
        public string Where;
        public List<string> Joins;
        public List<string> Orders;
        private string GetWhere()
        {
            if (Where is null)
            {
                return "";
            }
            return "WHERE " + Where;
        }
        private string GetJoins()
        {
            if (Joins is null)
            {
                return "";
            }
            return string.Join('\n', Joins);
        }
        private string GetOrders()
        {
            if (Orders is null)
            {
                return "";
            }
            return "ORDER BY " + string.Join(", ", Orders);
        }
        public override string ToString()
        {
            return $"SELECT {Select} FROM [{From}] {GetWhere()} {GetJoins()} {GetOrders()}".Trim();
        }
    }
    public struct SCommand
    {
        public int id { get; set; }
        public string database { get; set; }
        public string table { get; set; }
        public string name { get; set; }
        public string query { get; set; }
        public CommandType type { get; set; }
        public string restrictions { get; set; }

        public Command AsModel()
        {
            Command cmd = new();
            cmd.id = id;
            cmd.database = database;
            cmd.table = table;
            cmd.name = name;
            cmd.query = query;
            cmd.type = type;
            cmd.restrictions = restrictions;
            return cmd;
        }
    }

    public class Command : Model
    {
        public int id { get; set; }
        public string database { get; set; }
        public string table { get; set; }
        public string name { get; set; }
        public string query { get; set; }
        public CommandType type { get; set; }
        public string restrictions { get; set; }
        public Command()
        {
            tableName = "Commands";
        }
        public SCommand AsStruct()
        {
            SCommand result = new();
            result.id = id;
            result.database = database;
            result.table = table;
            result.name = name;
            result.query = query;
            result.type = type;
            result.restrictions = restrictions;
            return result;
        }
        private List<SCommand> SerializeList(DataTable dt)
        {
            List<SCommand> commands = new();
            foreach (DataRow dr in dt.Rows)
            {
                this.Serialize(dr);
                this.type = (CommandType)Enum.Parse(typeof(CommandType), dr["type"].ToString());
                commands.Add(this.AsStruct());
            }
            return commands;
        }
        public List<SCommand> GetCommandsOfTable(string db, string table)
        {
            DataTable dt = StartCommandToService()
                .Select()
                .WhereEqual(nameof(database), db)
                .WhereEqual(nameof(table), table)
                .Execute();
            return SerializeList(dt);
        }
        public List<SCommand> GetAllCommands()
        {
            DataTable dt = StartCommandToService()
                .Select()
                .Execute();
            return SerializeList(dt);
        }
        public void AddCommand()
        {
            StartCommandToService()
                .Insert(new()
                {
                    { nameof(database), database },
                    { nameof(table), table },
                    { nameof(name), name },
                    { nameof(query), query },
                    { nameof(type), type.ToString() },
                    { nameof(restrictions), restrictions },
                })
                .ExecuteVoid();
        }
    }
}
