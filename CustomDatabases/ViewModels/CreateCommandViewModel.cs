using Incas.Core.Classes;
using Incas.Core.Models;
using Incas.Core.ViewModels;
using Incas.CustomDatabases.Models;
using System.Collections.Generic;

namespace Incas.CustomDatabases.ViewModels
{
    public class CreateCommandViewModel : BaseViewModel
    {
        public CustomTable Requester = new();
        private Core.Models.Command command;

        public CreateCommandViewModel(Core.Models.Command cmd)
        {
            this.command = cmd;
        }
        public string CommandName
        {
            get => this.command.name;
            set
            {
                this.command.name = value;
                this.OnPropertyChanged(nameof(this.CommandName));
            }
        }
        public string Database => this.command.database;

        public string Table => this.command.table;
        public string Query
        {
            get => this.command.query;
            set
            {
                this.command.query = value;
                this.OnPropertyChanged(nameof(this.Query));
            }
        }
        public bool IsEditCommandType
        {
            get => this.command.type != CommandType.Read;
            set
            {
                this.command.type = value == true ? CommandType.Update : CommandType.Read;
                this.OnPropertyChanged(nameof(this.IsEditCommandType));
            }
        }
        public CommandType CommandType
        {
            get => this.command.type;
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
                List<string> unresolved =
                [
                    "UPDATE", "DELETE", "DROP", "SET", "CREATE", "%SELECTED%"
                ];
                foreach (string word in unresolved)
                {
                    if (this.Query.Contains(word))
                    {
                        DialogsManager.ShowExclamationDialog($"Обнаружено ключевое слово ({word}), " +
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
