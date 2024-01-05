using Models;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using Incubator_2.Windows;
using Incubator_2;
using System.Windows;
using Incubator_2.Models;
using System.Runtime.CompilerServices;


namespace Common
{
    public class LockedException : Exception
    {
        public LockedException() { }
    }
    public class UserAlreadyOnlineException : Exception
    {
        public UserAlreadyOnlineException() { }
    }
    public class SessionBrokenException : Exception 
    {
        public SessionBrokenException() { }
    }
    struct FirstWorkspaceData
    {
        public string workspacePath;
        public string workspaceName;
        public string userSurname;
        public string userFullname;
        public string userPassword;
    }

    static class ProgramState
    {
        public static string CommonPath { get; private set; }
        public static string UserPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Incubator";
        public static string DatabasePath { get { return CommonPath + @"\data.dbinc"; } }
        public static string CustomDatabasePath { get { return CommonPath + @"\custom.dbinc"; } }
        public static string ServiceDatabasePath { get { return Root + @"\service.dbinc"; } }
        private static string Root { get { return CommonPath + @"\Root"; } }
        public static string ServerProcesses { get { return Root + @"\ServerProccesses"; } } // ...\Root\ServerProccesses

        #region Templates

        private static string TemplatesPath { get { return Root + @"\Templates"; } }    // ...\Root\Templates
        public static string TemplatesSourcesWordPath { get { return TemplatesPath + @"\Sources\Word"; } }    // ...\Root\Templates\Sources\Word
        public static string TemplatesSourcesExcelPath { get { return TemplatesPath + @"\Sources\Excel"; } }    // ...\Root\Templates\Sources\Excel
        public static string TemplatesRuntime { get { return TemplatesPath + @"\Runtime"; } }    // ...\Root\Templates\Runtime
        public static string TemplatesGenerated { get { return TemplatesPath + @"\Generated"; } }    // ...\Root\Templates\Generated
        #endregion
        public static User CurrentUser { get; set; }

        public static Session CurrentSession { get; private set; }
        public static string SystemName = Environment.UserName;

        #region Path and init
        public static void SetCommonPath(string path)
        {
            CommonPath = path;
            Directory.CreateDirectory(TemplatesPath);
            Directory.CreateDirectory(TemplatesSourcesWordPath);
            Directory.CreateDirectory(TemplatesSourcesExcelPath);
            Directory.CreateDirectory(ServerProcesses);
            Directory.CreateDirectory(TemplatesRuntime);
            Directory.CreateDirectory(TemplatesGenerated);
        }
        public static void ClearDataForRestart()
        {
            CurrentUser = null;
            CurrentSession = null;
            CommonPath = null;
            Permission.CurrentUserPermission = PermissionGroup.Operator;
        }
        public static bool CheckSensitive()
        {
            if (CurrentUser == null || CurrentSession == null)
            {
                return false;
            }
            return true;
        }
        public static bool IsCommonPathExists()
        {
            return Directory.Exists(CommonPath);
        }
        private static string getDBFilePath()
        {
            return CommonPath + @"\data.dbinc";
        }
        public static void GetDBFile()
        {
            if (!File.Exists(getDBFilePath()))
            {
                Dialog q = new Dialog("По указанному пути рабочее пространство не обнаружено.\n" +
                "Проверьте правильность введенного пути или создайте новое рабочее пространство.", "Рабочее пространство не обнаружено");
                q.ShowDialog();
            }
        }
        #endregion

        #region ComputerId
        public static string GenerateSlug(int len, string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
        {
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, len)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private static bool IsRegistryContainsData()
        {
            return Registry.CurrentUser.GetSubKeyNames().Contains("Incubator");
        }

        #endregion

        #region UserData
        public static bool LoadUserData() 
        {
            if (IsRegistryContainsData())
            {
                SetCommonPath(RegistryData.GetSelectedWorkspacePath());
                return true;
            }
            return false;
        }
        #endregion

        #region WorkspaceFiles
        public static string GetFullnameOfWordFile(string name)
        {
            return TemplatesSourcesWordPath + "\\" + name;
        }
        #endregion
        private static bool CreateTablesInDatabase()
        {
            return DatabaseManager.CreateTables();
        }
        public static Parameter GetParameter(ParameterType type, string name, string defaultValue="0", bool createIfNot = true)
        {
            Parameter par = new Parameter();
            par.GetParameter(type, name, defaultValue, createIfNot);
            return par;
        }

        #region Incubator
        public static void InitWorkspace(FirstWorkspaceData data)
        {
            Permission.CurrentUserPermission = PermissionGroup.Admin;
            SetCommonPath(data.workspacePath);
            CreateTablesInDatabase();
            RegistryData.SetWorkspacePath(data.workspaceName, data.workspacePath);
            using (Parameter par = new Parameter())
            {
                par.type = ParameterType.INCUBATOR;
                par.name = "ws_name";
                par.value = data.workspaceName;
                par.CreateParameter();
                par.name = "ws_opened";
                par.WriteBoolValue(true);
                par.CreateParameter();
                par.name = "ws_locked";
                par.WriteBoolValue(false);
                par.CreateParameter();
            }
            using (User user = new User())
            {
                user.status = PermissionGroup.Admin;
                user.username = "admin";
                user.post = "Администратор рабочего пространства";
                user.surname = data.userSurname;
                user.fullname = data.userFullname;
                user.password = data.userPassword;
                user.AddUser();
            }

        }
        public static string GetWorkspaceName()
        {
            using (Parameter par = GetParameter(ParameterType.INCUBATOR, "ws_name", "Рабочая область"))
            {
                return par.value;
            }
        }
        public static void SetWorkspaceName(string name)
        {
            using (Parameter par = GetParameter(ParameterType.INCUBATOR, "ws_name", "Рабочая область"))
            {
                par.value = name;
                par.UpdateValue();
            }
        }
        public static void SetWorkspaceOpened(bool opened)
        {
            using (Parameter par = GetParameter(ParameterType.INCUBATOR, "ws_opened", "1"))
            {
                par.WriteBoolValue(opened);
                par.UpdateValue();
            }
        }
        public static bool IsWorkspaceOpened()
        {
            if (Permission.CurrentUserPermission != PermissionGroup.Admin)
            {
                using (Parameter par = GetParameter(ParameterType.INCUBATOR, "ws_opened", "1"))
                {
                    bool result = par.GetValueAsBool();
                    if (!result)
                    {
                        ShowErrorDialog("Действия по добавлению, изменению " +
                            "или удалению информации из базы данных недоступны, " +
                            "пока рабочее пространство находится в статусе \"Закрыто\".\n" +
                            "Рабочее пространство по-прежнему можно использовать, однако " +
                            "только для чтения.", "Рабочее пространство закрыто");
                    }
                    return result;
                }
            }
            else
            {
                return true;
            }
        }
        public static void SetWorkspaceLocked(bool locked)
        {
            using (Parameter par = GetParameter(ParameterType.INCUBATOR, "ws_locked"))
            {
                par.WriteBoolValue(locked);
                par.UpdateValue();
            } 
        }
        public static bool IsWorkspaceLocked()
        {
            using (Parameter par = GetParameter(ParameterType.INCUBATOR, "ws_locked"))
            {
                return par.GetValueAsBool();
            }
        }
        
        #endregion
        public static void CheckLocked()
        {
            if (IsWorkspaceLocked() && Permission.CurrentUserPermission != PermissionGroup.Admin)
            {
                throw new LockedException();
            }
        }

        #region Session

        public static void BrokeSession()
        {
            throw new SessionBrokenException();
        }
        public static void OpenSession()
        {
            Session ms = new Session();    
            ms.AddSession();
            CurrentSession = ms; 
        }
        public static void CloseSession() 
        {
            if (CurrentSession != null && CurrentSession.active)
            {
                CurrentSession.CloseSession();
            }
        }
        #endregion

        #region ModalDialogs
        public static void ShowErrorDialog(string message, string title = "Возникла неизвестная ошибка")
        {
            Dialog d = new Dialog(message, title);
            d.ShowDialog();
        }
        public static void ShowExlamationDialog(string message, string title = "Обратите внимание")
        {
            Dialog d = new Dialog(message, title, Dialog.DialogIcon.Exlamation);
            d.ShowDialog();
        }
        public static DialogStatus ShowQuestionDialog(string message, string title, string yesText="Да", string noText = "Нет")
        {
            DialogQuestion d = new DialogQuestion(message, title, yesText, noText);
            d.ShowDialog();
            return d.status;
        }
        #endregion

        public async static void ClearRuntimeFiles()
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                foreach (string item in Directory.GetFiles(ProgramState.TemplatesRuntime))
                {
                    try
                    {
                        File.Delete(item);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            });
        }
    }
}
