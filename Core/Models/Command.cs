using Common;
using IncasEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace Incas.Core.Models
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
            Command cmd = new()
            {
                id = this.id,
                database = this.database,
                table = this.table,
                name = this.name,
                query = this.query,
                type = this.type,
                restrictions = this.restrictions
            };
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
            this.tableName = "Commands";
        }
        public SCommand AsStruct()
        {
            SCommand result = new()
            {
                id = this.id,
                database = this.database,
                table = this.table,
                name = this.name,
                query = this.query,
                type = this.type,
                restrictions = this.restrictions
            };
            return result;
        }
        private List<SCommand> SerializeList(DataTable dt)
        {
            List<SCommand> commands = [];
            foreach (DataRow dr in dt.Rows)
            {
                this.Serialize(dr);
                this.type = (CommandType)Enum.Parse(typeof(CommandType), dr["type"].ToString());
                this.DecryptQuery();
                commands.Add(this.AsStruct());
            }
            return commands;
        }
        public List<SCommand> GetCommandsOfTable(string db, string table)
        {
            DataTable dt = this.StartCommandToService()
                .Select()
                .WhereEqual(nameof(this.database), db)
                .WhereEqual(nameof(table), table)
                .OrderByASC(nameof(this.name))
                .Execute();
            return this.SerializeList(dt);
        }

        public List<SCommand> GetAllCommands()
        {
            DataTable dt = this.StartCommandToService()
                .Select()
                .Execute();
            return this.SerializeList(dt);
        }
        public void AddCommand()
        {
            if (this.id > 0)
            {
                this.Update();
                return;
            }
            this.StartCommandToService()
                .Insert(new()
                {
                    { nameof(this.database), this.database },
                    { nameof(this.table), this.table },
                    { nameof(this.name), this.name },
                    { nameof(this.query), this.EncryptQuery() },
                    { nameof(this.type), this.type.ToString() },
                    { nameof(this.restrictions), this.restrictions },
                })
                .ExecuteVoid();
        }
        public void Update()
        {
            this.StartCommandToService()
                .Update(nameof(this.name), this.name)
                .Update(nameof(this.query), this.EncryptQuery())
                .Update(nameof(this.type), this.type.ToString())
                .Update(nameof(this.restrictions), this.restrictions)
                .WhereEqual(nameof(this.id), this.id)
                .ExecuteVoid();
        }
        public string EncryptQuery()
        {
            return Cryptographer.EncryptString(Cryptographer.GenerateKey(this.database + this.table), this.query);
        }
        public void DecryptQuery()
        {
            this.query = Cryptographer.DecryptString(Cryptographer.GenerateKey(this.database + this.table), this.query);
        }
        public void DeleteCommand()
        {
            this.StartCommandToService()
                .Delete()
                .WhereEqual(nameof(this.id), this.id)
                .ExecuteVoid();
        }
    }
}
