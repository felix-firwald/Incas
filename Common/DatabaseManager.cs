using Models;
using System.Data;

using System.Data.SQLite;

namespace Common
{
    public static class DatabaseManager // static cause server connection must be only one per session
    {
        readonly public static string path = ProgramState.DatabasePath;
        public static SQLiteConnection connection;
        public static void OpenConnection()
        {
            connection = new SQLiteConnection($"Data source={path}; Version=3; datetimeformat=CurrentCulture");
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }
        public static void CloseConnection()
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public static bool CreateTables(string path)
        {
            SQLiteConnection.CreateFile(path);
            using (Models.Incubator i = new Models.Incubator())
            {
                i.CreateDefinition();
                i.InitializeIncubator();
            }
            using (Post p = new Post())
            {
                p.CreateDefinition();
                p.name = "Администратор инкубатора";
                p.permission = PermissionGroup.Admin;
                p.rights = "ALL";
            }
            using (User s = new User())
            {
                s.CreateDefinition();
                s.username = "admin";
                s.password = "1234";
                s.post = "Администратор инкубатора";
                s.status = PermissionGroup.Admin;
                s.AddUser();
            }
            using (Computer s = new Computer())
            {
                s.CreateDefinition();
            }
            using (Session s = new Session())
            {
                s.CreateDefinition();
            }
            using (Task s = new Task())
            {
                s.CreateDefinition();
            }
            using (Subtask s = new Subtask())
            {
                s.CreateDefinition();
            }
            using (Enumeration s = new Enumeration())
            {
                s.CreateDefinition();
            }
            using (Template s = new Template())
            {
                s.CreateDefinition();
            }
            using (Tag s = new Tag())
            {
                s.CreateDefinition();
            }
            using (Constant s = new Constant())
            {
                s.CreateDefinition();
            }
            return true;
        }
    }
}

