using Common;
using Incubator_2.Models;
using Incubator_2.Windows.CustomDatabase;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.Common
{
    public static class CommandHandler
    {
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
                ProgramState.ShowErrorDialog("Обнаружена синтаксическая ошибка! Команда не будет выполнена.");
            }
        }
        private static void SwitchOnAction(string action, string arguments)
        {
            switch (action)
            {
                case "#new":
                    SwitchOnCreationCommands(arguments);
                    break;
                case "#remove":
                    SwitchOnDeleteCommands(arguments);
                    break;
                case "#restrict":
                    SwitchOnRestrictCommands(arguments);
                    break;
                case "#lock":
                    ProgramState.SetWorkspaceLocked(true);
                    ProgramState.ShowInfoDialog("Рабочее пространство успешно заблокировано", "Отчет о выполнении");
                    break;
                case "#unlock":
                    ProgramState.SetWorkspaceLocked(false);
                    ProgramState.ShowInfoDialog("Рабочее пространство успешно разблокировано", "Отчет о выполнении");
                    break;
                case "#open":
                    ProgramState.SetWorkspaceOpened(true);
                    ProgramState.ShowInfoDialog("Рабочее пространство успешно открыто", "Отчет о выполнении");
                    break;
                case "#close":
                    ProgramState.SetWorkspaceOpened(false);
                    ProgramState.ShowInfoDialog("Рабочее пространство успешно закрыто", "Отчет о выполнении");
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
                    ProgramState.ShowExclamationDialog("Команда не существует!");
                    break;
            }
        }
        private static void SwitchOnDeleteCommands(string input)
        {

        }
        private static void SwitchOnRestrictCommands(string input)
        {
            input = input.Trim();
            switch (input)
            {
                case "edit table":
                case "editing table":
                    using (Parameter p = new())
                    {
                        p.type = ParameterType.RESTRICT_EDIT_TABLE;
                        BindingSelector bs = ProgramState.ShowBindingSelector();
                        if (bs.Result == Windows.DialogStatus.Yes)
                        {
                            p.name = $"{bs.SelectedDatabase}.{bs.SelectedTable}";
                            User u = ProgramState.ShowUserSelector();
                            p.value = u.id.ToString();
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
                        if (bs.Result == Windows.DialogStatus.Yes)
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
                    ProgramState.ShowExclamationDialog("Команда не существует!");
                    break;
            }
        }
    }
}
