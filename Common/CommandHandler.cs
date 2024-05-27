using Common;
using Incas.Core.Views.Windows;
using Incubator_2.Models;
using Incas.Users.Views.Windows;
using Incubator_2.Windows.CustomDatabase;
using System;
using Incas.Users.Models;

namespace Incas.Common
{
    public static class CommandHandler
    {
        private static void ShowObjectNotFound(string commandName, string obj)
        {
            ProgramState.ShowExclamationDialog($"Объект или модификатор (\"{obj}\") для команды #{commandName} не предусмотрены!", "Команда не будет выполнена");
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
                ProgramState.ShowErrorDialog("Обнаружена синтаксическая ошибка!", "Команда не будет выполнена");
            }
        }
        private static void SwitchOnAction(string action, string arguments)
        {
            switch (action)
            {

                case "#lock":
                case "#set locked 1":
                    ProgramState.SetWorkspaceLocked(true);
                    ProgramState.ShowInfoDialog("Рабочее пространство успешно заблокировано", "Отчет о выполнении");
                    break;
                case "#unlock":
                case "#set locked 0":
                    ProgramState.SetWorkspaceLocked(false);
                    ProgramState.ShowInfoDialog("Рабочее пространство успешно разблокировано", "Отчет о выполнении");
                    break;
                case "#open":
                case "#set opened 1":
                    ProgramState.SetWorkspaceOpened(true);
                    ProgramState.ShowInfoDialog("Рабочее пространство успешно открыто", "Отчет о выполнении");
                    break;
                case "#close":
                case "#set opened 0":
                    ProgramState.SetWorkspaceOpened(false);
                    ProgramState.ShowInfoDialog("Рабочее пространство успешно закрыто", "Отчет о выполнении");
                    break;
                case "#new":
                    SwitchOnCreationCommands(arguments);
                    break;
                case "#drop":
                    SwitchOnDeleteCommands(arguments);
                    break;
                case "#restrict":
                case "#set restriction":
                    SwitchOnRestrictCommands(arguments);
                    break;
                case "#set":

                    break;
                default:
                    ProgramState.ShowExclamationDialog("Команда не существует!");
                    break;
            }
        }
        private static void SwitchOnCreationCommands(string input)
        {
            input = input.Trim();
            switch (input)
            {
                case "sector":
                    using (Sector sector = new())
                    {
                        sector.name = ProgramState.ShowInputBox("Имя сектора", "Введите имя сектора");
                        sector.AddSector();
                    }
                    break;
                case "user":
                    UserEditor ue = new(new User());
                    ue.ShowDialog();
                    break;
                case "database":
                    using (Database db = new())
                    {
                        db.name = ProgramState.ShowInputBox("База данных", "Введите имя базы данных");
                        using (Sector sector = new())
                        {
                            db.sectors = string.Join(" ", sector.GetSectorSlugs());
                        }
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
                    if (ProgramState.ShowUserSelector(out u))
                    {
                        u.RemoveUser();
                    }
                    break;
            }
        }
        private static void SwitchOnRestrictCommands(string input)
        {
            input = input.Trim();
            switch (input)
            {
                case "table administrator":
                case "table moderator":
                    using (Parameter p = new())
                    {
                        p.type = ParameterType.RESTRICT_EDIT_TABLE;
                        BindingSelector bs = ProgramState.ShowBindingSelector();
                        if (bs.Result == DialogStatus.Yes)
                        {
                            p.name = $"{bs.SelectedDatabase}.{bs.SelectedTable}";
                            p.DeleteParameterByTypeAndName();
                            p.value = input.Split(' ')[1];
                            p.CreateParameter();
                        }
                        else
                        {
                            return;
                        }

                    }
                    break;
                case "nullify table":
                case "nullify edit table":
                case "nullify editing table":
                    using (Parameter p = new())
                    {
                        p.type = ParameterType.RESTRICT_EDIT_TABLE;
                        BindingSelector bs = ProgramState.ShowBindingSelector();
                        if (bs.Result == DialogStatus.Yes)
                        {
                            p.name = $"{bs.SelectedDatabase}.{bs.SelectedTable}";
                            p.DeleteParameterByTypeAndName();
                        }
                        else
                        {
                            return;
                        }
                    }
                    break;
                default:
                    ShowObjectNotFound("restrict", input);
                    break;
            }
        }
    }
}
