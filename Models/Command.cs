using Common;
using Incubator_2.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace Incubator_2.Models
{
    
    public enum CommandType
    {
        [Description("Чтение")]
        Read,
        [Description("Изменение")]
        Update,
        [Description("Для шаблонов: трансфер")]
        TemplateTransfer,
        [Description("Для шаблонов: парсер из строки")]
        TemplateParser,
        [Description("Для шаблонов: триггеры")]
        TemplateTrigger,
        [Description("Для шаблонов: команды")]
        TemplateCommand,
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
                DecryptQuery();
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
                .OrderByASC(nameof(name))
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
            if (id > 0)
            {
                Update();
                return;
            }
            StartCommandToService()
                .Insert(new()
                {
                    { nameof(database), database },
                    { nameof(table), table },
                    { nameof(name), name },
                    { nameof(query), EncryptQuery() },
                    { nameof(type), type.ToString() },
                    { nameof(restrictions), restrictions },
                })
                .ExecuteVoid();
        }
        public void Update()
        {
            StartCommandToService()
                .Update(nameof(name), name)
                .Update(nameof(query), EncryptQuery())
                .Update(nameof(type), type.ToString())
                .Update(nameof(restrictions), restrictions)
                .WhereEqual(nameof(id), id)
                .ExecuteVoid();
        }
        public string EncryptQuery()
        {
            return Cryptographer.EncryptString(Cryptographer.GenerateKey(database + table), query);
        }
        public void DecryptQuery()
        {
            query = Cryptographer.DecryptString(Cryptographer.GenerateKey(database + table), query);
        }
        public void DeleteCommand()
        {
            StartCommandToService()
                .Delete()
                .WhereEqual(nameof(id), id)
                .ExecuteVoid();
        }
    }
}
