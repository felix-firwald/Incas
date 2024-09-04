using Incas.Core.Models;
using Incas.Core.Views.Windows;
using Incas.CustomDatabases.Models;
using Incas.CustomDatabases.Views.Windows;
using Incas.Users.Models;
using Incas.Users.Views.Windows;
using System;

namespace Incas.Core.Classes
{
    public static class CommandHandler
    {
        private static void ShowObjectNotFound(string commandName, string obj)
        {
            DialogsManager.ShowExclamationDialog($"Объект или модификатор (\"{obj}\") для команды #{commandName} не предусмотрены!", "Команда не будет выполнена");
        }
        public static void Handle(string input)
        {
            try
            {
                input = input.Trim();
                string command = input.Split(' ')[0];
                SwitchOnAction(command, input.Replace(command, ""));
            }
            catch (Exception)
            {
                DialogsManager.ShowErrorDialog("Обнаружена синтаксическая ошибка!", "Команда не будет выполнена");
            }
        }
        private static void SwitchOnAction(string action, string arguments)
        {
            switch (action)
            {

                case "#lock":
                case "#set locked 1":
                    ProgramState.SetWorkspaceLocked(true);
                    DialogsManager.ShowInfoDialog("Рабочее пространство успешно заблокировано", "Отчет о выполнении");
                    break;
                case "#unlock":
                case "#set locked 0":
                    ProgramState.SetWorkspaceLocked(false);
                    DialogsManager.ShowInfoDialog("Рабочее пространство успешно разблокировано", "Отчет о выполнении");
                    break;
                case "#open":
                case "#set opened 1":
                    ProgramState.SetWorkspaceOpened(true);
                    DialogsManager.ShowInfoDialog("Рабочее пространство успешно открыто", "Отчет о выполнении");
                    break;
                case "#close":
                case "#set opened 0":
                    ProgramState.SetWorkspaceOpened(false);
                    DialogsManager.ShowInfoDialog("Рабочее пространство успешно закрыто", "Отчет о выполнении");
                    break;
                case "#new":
                    SwitchOnCreationCommands(arguments);
                    break;
                case "#drop":
                    SwitchOnDeleteCommands(arguments);
                    break;
                case "#set":

                    break;
                default:
                    DialogsManager.ShowExclamationDialog("Команда не существует!");
                    break;
            }
        }
        private static void SwitchOnCreationCommands(string input)
        {
            input = input.Trim();
            switch (input)
            {
                case "database":
                    using (Database db = new())
                    {
                        db.name = DialogsManager.ShowInputBox("База данных", "Введите имя базы данных");
                        db.AddDatabase();
                    }
                    break;
                default:
                    ShowObjectNotFound("new", input);
                    break;
            }
        }
        private static void SwitchOnDeleteCommands(string input)
        {
            input = input.Trim();
            switch (input)
            {
                case "user":
                    User u;
                    if (DialogsManager.ShowUserSelector(out u))
                    {
                        u.RemoveUser();
                    }
                    break;
            }
        }
    }
}
