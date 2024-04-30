using Common;
using Incubator_2.Models;
using System.Collections.Generic;

namespace Incubator_2.ViewModels.VM_CustomDB
{
    public class VM_CreateCommand : VM_Base
    {
        public CustomTable Requester = new();
        private Models.Command command;

        public VM_CreateCommand(Models.Command cmd)
        {
            this.command = cmd;
        }
        public string CommandName
        {
            get
            {
                return this.command.name;
            }
            set
            {
                this.command.name = value;
                this.OnPropertyChanged(nameof(this.CommandName));
            }
        }
        public string Database
        {
            get
            {
                return this.command.database;
            }
        }

        public string Table
        {
            get
            {
                return this.command.table;
            }
        }
        public string Query
        {
            get
            {
                return this.command.query;
            }
            set
            {
                this.command.query = value;
                this.OnPropertyChanged(nameof(this.Query));
            }
        }
        public bool IsEditCommandType
        {
            get
            {
                if (this.command.type == CommandType.Read)
                {
                    return false;
                }
                return true;
            }
            set
            {
                if (value == true)
                {
                    this.command.type = CommandType.Update;
                }
                else
                {
                    this.command.type = CommandType.Read;
                }
                this.OnPropertyChanged(nameof(this.IsEditCommandType));
            }
        }
        public CommandType CommandType
        {
            get
            {
                return this.command.type;
            }
            set
            {
                this.command.type = value;
                this.OnPropertyChanged(nameof(this.CommandType));
            }
        }
        public string GetPKField()
        {
            using CustomTable ct = new();
            return ct.GetPKField(this.Table, this.Database);
        }
        public bool ValidateQuery()
        {
            if (!this.IsEditCommandType)
            {
                List<string> unresolved = new()
                {
                    "UPDATE", "DELETE", "DROP", "SET", "CREATE", "%SELECTED%"
                };
                foreach (string word in unresolved)
                {
                    if (this.Query.Contains(word))
                    {
                        ProgramState.ShowExclamationDialog($"Обнаружено ключевое слово ({word}), " +
                            "которое не может быть использовано в режиме чтения", "Сохранение прервано");
                        return false;
                    }
                }
            }
            return true;
        }
        public void Save()
        {
            this.command.AddCommand();
        }
    }
}
