using Common;
using Incubator_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.UserDataTasks;

namespace Incubator_2.ViewModels.VM_CustomDB
{
    public class VM_CreateCommand : VM_Base
    {
        public CustomTable Requester = new();
        private Models.Command command;

        public VM_CreateCommand(Models.Command cmd)
        {
            command = cmd;
        }
        public string CommandName
        {
            get
            {
                return command.name;
            }
            set
            {
                command.name = value;
                OnPropertyChanged(nameof(CommandName));
            }
        }
        public string Database
        {
            get
            {
                return command.database;
            }
        }

        public string Table
        {
            get
            {
                return command.table;
            }
        }
        public string Query
        {
            get
            {
                return command.query;
            }
            set
            {
                command.query = value;
                OnPropertyChanged(nameof(Query));
            }
        }
        public bool IsEditCommandType
        {
            get
            {
                if (command.type == CommandType.Read)
                {
                    return false;
                }
                return true;
            }
            set
            {
                if (value == true)
                {
                    command.type = CommandType.Update;
                }
                else
                {
                    command.type = CommandType.Read;
                }
                OnPropertyChanged(nameof(IsEditCommandType));
            }
        }
        public CommandType CommandType
        {
            get
            {
                return command.type;
            }
            set
            {
                command.type = value;
                OnPropertyChanged(nameof(CommandType));
            }
        }
        public string GetPKField()
        {
            using (CustomTable ct = new())
            {
                return ct.GetPKField(Table, Database);
            }
        }
        public bool ValidateQuery()
        {
            if (!IsEditCommandType)
            {
                List<string> unresolved = new()
                {
                    "UPDATE", "DELETE", "DROP", "SET", "CREATE", "%SELECTED%"
                };
                foreach (string word in unresolved)
                {
                    if (Query.Contains(word))
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
            command.AddCommand();
        }
    }
}
