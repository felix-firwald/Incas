using Models;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using Incubator_2.Windows;
using Incubator_2;
using System.Windows;
using Incubator_2.Models;


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

    static class ProgramState
    {
        public static string CommonPath { get; private set; }
        public static string UserPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Incubator";
        public static string DatabasePath { get; private set; }
        public static string CustomDatabasePath { get; private set; }
        public static string ProceduralPath { get; private set; }

        #region Templates
        private static string TemplatesPath { get; set; }    // ...\Templates
        public static string TemplatesSourcesWordPath { get; private set; }    // ...\Templates\Sources\Word
        public static string TemplatesSourcesExcelPath { get; private set; }    // ...\Templates\Sources\Excel
        public static string TemplatesRuntime { get; private set; }    // ...\Templates\Runtime
        public static string TemplatesGenerated { get; private set; }    // ...\Templates\Generated
        #endregion
        public static string User { get; set; }

        public static Session CurrentSession { get; set; }
        public static string SystemName = Environment.UserName;

        #region Path and init
        public static void SetCommonPath(string path)
        {
            CommonPath = path;
            DatabasePath = CommonPath + @"\data.dbinc";
            CustomDatabasePath = CommonPath + @"\custom.dbinc";
            TemplatesPath = CommonPath + @"\Templates";
            ProceduralPath = CommonPath + @"\Procedural";
            Directory.CreateDirectory(TemplatesPath);
            string sourcePath = TemplatesPath + @"\Sources";
            TemplatesSourcesWordPath = sourcePath + @"\Word";
            TemplatesSourcesExcelPath = sourcePath + @"\Excel";
            TemplatesRuntime = TemplatesPath + @"\Runtime";
            TemplatesGenerated = TemplatesPath + @"\Generated";
            Directory.CreateDirectory(TemplatesSourcesWordPath);
            Directory.CreateDirectory(TemplatesSourcesExcelPath);
            Directory.CreateDirectory(ProceduralPath);
            Directory.CreateDirectory(TemplatesRuntime);
            Directory.CreateDirectory(TemplatesGenerated);
        }
        public static bool IsCommonPathExists()
        {
            return Directory.Exists(CommonPath);
        }
        private static string getInitFilePath()
        {
            return CommonPath + @"\init.incb";
        }
        public static string GetInitFile()
        {
            string fileFullName = getInitFilePath();
            if (!File.Exists(fileFullName))
            {
                DialogQuestion q = new DialogQuestion("По указанному пути рабочее пространство не обнаружено.\n" +
                "Проверьте правильность введенного пути или создайте новое рабочее пространство " +
                "(будет открыт конструктор нового рабочего пространства)", "Рабочее пространство не обнаружено", "Создать новое", "Не создавать");
                q.ShowDialog();
                if (q.status == DialogStatus.Yes)
                {
                    Initialize();
                }
                else
                {
                    Application.Current.Run();
                }
            }
            return File.ReadAllText(fileFullName);
        }
        #endregion

        #region ComputerId
        public static string GenerateSlug(int len, string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
        {
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, len)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private static string GenerateComputerId()
        {
            Random random = new Random();
            //const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string result = GenerateSlug(20);
            RegistryKey inc = Registry.CurrentUser.CreateSubKey("Incubator");
            inc.SetValue("computer", result);
            inc.CreateSubKey("TemplatesData");
            inc.Close();
            using (Computer mc = new Computer())
            {
                mc.authId = result;
                mc.name = SystemName;
                mc.blocked = false;
                mc.AddComputer();
            }
            return result;
        }
        public static string GetComputerId()
        {
            if (IsRegistryContainsData()) 
            {
                string result = RegistryData.GetRoot().GetValue("computer").ToString();  
                return result;
            }
            return GenerateComputerId();
        }
        private static bool IsRegistryContainsData()
        {
            return Registry.CurrentUser.GetSubKeyNames().Contains("Incubator");
        }

        public static void RegisterComputer()
        {
            using (Computer mc = new Computer())
            {
                mc.authId = GetComputerId();
                mc.blocked = false;
                mc.AddComputer();
            }
        }
        #endregion

        #region UserData
        public static void SaveUserData()
        {
            if (!IsRegistryContainsData())
            {
                GenerateComputerId();
            }
            RegistryKey inc = Registry.CurrentUser.OpenSubKey("Incubator", true);
            inc.SetValue("path", CommonPath);
            inc.Close();
        }
        public static bool LoadUserData() 
        {
            if (IsRegistryContainsData())
            {
                //SetCommonPath(File.ReadAllText(UserPath + @"\path.incdat").Trim());
                SetCommonPath(RegistryData.GetSelectedWorkspacePath());
                return true;
            }
            return false;
        }
        public static User GetCurrentUser()
        {
            User user = new User();
            user.username = User;
            return user.GetUserByName();
        }
        #endregion

        #region WorkspaceFiles
        public static string GetFullnameOfWordFile(string name)
        {
            return TemplatesSourcesWordPath + "\\" + name;
        }
        #endregion
        private static bool Initialize()
        {
            string fileFullName = getInitFilePath();
            File.WriteAllText(fileFullName, SystemName);
            Permission.CurrentUserPermission = PermissionGroup.Admin;
            return CreateTablesInDatabase();
        }
        private static bool CreateTablesInDatabase()
        {
            return DatabaseManager.CreateTables(DatabasePath);
        }
        public static Parameter GetParameter(ParameterType type, string name, string defaultValue="0", bool createIfNot = true)
        {
            Parameter par = new Parameter();
            par.GetParameter(type, name, defaultValue, createIfNot);
            return par;
        }

        #region Incubator
        public static void InitWorkspace(string workspaceName)
        {
            using (Parameter par = new Parameter())
            {
                par.type = ParameterType.INCUBATOR;
                par.name = "ws_name";
                par.value = workspaceName;
                par.CreateParameter();
                par.name = "ws_opened";
                par.WriteBoolValue(true);
                par.CreateParameter();
                par.name = "ws_locked";
                par.WriteBoolValue(false);
                par.CreateParameter();
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
        public static void LoadUserStatus()
        {
            using (User mu = new User())
            {
                mu.username = User;
                Permission.CurrentUserPermission = mu.GetUserByName().status;
            }
        }
        #region Session
        public async static void CheckSession()
        {
            if (Permission.CurrentUserPermission != PermissionGroup.Admin)
            {
                string searchingPath = $"{ProceduralPath}\\kill_{CurrentSession.id}.incproc";
                await System.Threading.Tasks.Task.Run(() =>
                {
                    while (true)
                    {
                        System.Threading.Tasks.Task.Delay(3000).Wait();
                        if (File.Exists(searchingPath) && File.ReadAllText(searchingPath) == GetComputerId())
                        {
                            File.Delete(searchingPath);
                            throw new Exception("Сессия принудительно завершена администратором инкубатора");
                        }
                    }
                });
            }          
        }
        public static void GenerateKillerFile(string sessionId, string computerId)
        {
            string pathOfFile = $"{ProceduralPath}\\kill_{sessionId}.incproc";
            File.WriteAllText(pathOfFile, computerId);
        }
        public static void BrokeSession()
        {
            throw new SessionBrokenException();
        }
        public static void OpenSession()
        {
            using (Session ms = new Session())
            {
                ms.AddSession();
            }   
        }
        public static void CloseSession() 
        {
            CurrentSession.CloseSession();
        }
        #endregion

        #region ModalDialogs
        public static void ShowErrorDialog(string message, string title = "Возникла неизвестная ошибка")
        {
            Dialog d = new Dialog(message, title);
            d.ShowDialog();
        }
        public static DialogStatus ShowQuestionDialog(string message, string title, string yesText="Да", string noText = "Нет")
        {
            DialogQuestion d = new DialogQuestion(message, title, yesText, noText);
            d.ShowDialog();
            return d.status;
        }
        #endregion
    }
}
