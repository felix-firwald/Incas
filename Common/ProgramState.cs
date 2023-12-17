using Models;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using Incubator_2.Windows;
using Incubator_2;
using System.Windows;


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
        public static string TemplatesPath { get; private set; }    // ...\Templates
        public static string TemplatesSourcesWordPath { get; private set; }    // ...\Templates\Sources\Word
        public static string TemplatesSourcesExcelPath { get; private set; }    // ...\Templates\Sources\Excel
        #endregion
        public static string User { get; set; }

        public static Session CurrentSession { get; set; }
        public static string SystemName = Environment.UserName;

        #region Path and init
        public static void SetCommonPath(string path)
        {
            CommonPath = path;
            DatabasePath = path + @"\data.dbinc";
            CustomDatabasePath = path + @"\custom.dbinc";
            TemplatesPath = path + @"\Templates";
            ProceduralPath = path + @"\procedural";
            Directory.CreateDirectory(TemplatesPath);
            string sourcePath = TemplatesPath + @"\Sources";

            TemplatesSourcesWordPath = sourcePath + @"\Word";
            TemplatesSourcesExcelPath = sourcePath + @"\Excel";
            Directory.CreateDirectory(TemplatesSourcesWordPath);
            Directory.CreateDirectory(TemplatesSourcesExcelPath);
            Directory.CreateDirectory(ProceduralPath);
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
                string result = Registry.CurrentUser.OpenSubKey("Incubator").GetValue("computer").ToString();  
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
                SetCommonPath(Registry.CurrentUser.OpenSubKey("Incubator").GetValue("path").ToString());
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
        private static bool Initialize()
        {
            string fileFullName = getInitFilePath();
            File.WriteAllText(fileFullName, SystemName);
            Console.WriteLine(DatabasePath);
            Permission.CurrentUserPermission = PermissionGroup.Admin;
            return CreateTablesInDatabase();
        }
        private static bool CreateTablesInDatabase()
        {
            return DatabaseManager.CreateTables(DatabasePath);
        }

        #region Incubator
        public static Models.Incubator GetIncubatorInfo()
        {
            using (Models.Incubator mi = new Models.Incubator())
            {
                return mi.GetIncubator();
            }
        }
        public static void SetIncubatorOpened(bool opened)
        {
            Models.Incubator mi = GetIncubatorInfo();
            mi.opened = opened;
            mi.UpdateIncubator();
        }
        public static bool IsIncubatorOpened()
        {
            return GetIncubatorInfo().opened;
        }
        public static void SetIncubatorLocked(bool locked)
        {
            Models.Incubator mi = GetIncubatorInfo();
            mi.locked = locked;
            mi.UpdateIncubator();
        }
        public static bool IsIncubatorLocked()
        {
            return GetIncubatorInfo().locked;
        }
        public static void RenameIncubator(string newName)
        {
            Models.Incubator mi = GetIncubatorInfo();
            mi.name = newName;
            mi.UpdateIncubator();
        }
        public static void SetRepositoryEnabled(bool enabled)
        {
            Models.Incubator mi = GetIncubatorInfo();
            mi.repositoryEnabled = enabled;
            mi.UpdateIncubator();
        }
        public static bool CheckIncubatorOpened()
        {
            if (!ProgramState.IsIncubatorOpened())
            {
                new Dialog("Действия по добавлению, изменению или удалению информации из базы данных недоступны, пока инкубатор находится в статусе \"Закрыт\"", "Инкубатор закрыт");
                return false;
            }
            return true;
        }
        #endregion
        public static void CheckLocked()
        {
            if (IsIncubatorLocked() && Permission.CurrentUserPermission != PermissionGroup.Admin)
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
    }
}
