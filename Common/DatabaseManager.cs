using Models;
using System.Collections.Generic;
using System.Data;

using System.Data.SQLite;
using System.Windows.Markup;

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

        private static string[] GetIncubatorDefinition()
        {
            return new string[] 
            {
                "id INTEGER PRIMARY KEY AUTOINCREMENT",
                "name STRING NOT NULL",
                "opened BOOLEAN NOT NULL DEFAULT (1)",
                "locked BOOLEAN NOT NULL DEFAULT (0)"
            };
        }
        private static string[] GetPostDefinition()
        {
            return new string[]
            {
                "id INTEGER PRIMARY KEY AUTOINCREMENT",
                "name STRING UNIQUE ON CONFLICT ROLLBACK NOT NULL DEFAULT [Администратор инкубатора]",
                "permission STRING NOT NULL",
                "rights STRING NOT NULL",
            };
        }
        private static string[] GetUserDefinition()
        {
            return new string[]
            {
                "id INTEGER PRIMARY KEY AUTOINCREMENT",
                "username STRING UNIQUE ON CONFLICT ROLLBACK NOT NULL",
                "fullname STRING",
                "surname STRING",
                "post INTEGER REFERENCES Posts (id)",
                "password STRING UNIQUE ON CONFLICT ROLLBACK NOT NULL",
                "status STRING NOT NULL",
            };
        }
        private static string[] GetComputerDefinition()
        {
            return new string[]
            {
                "id INTEGER PRIMARY KEY AUTOINCREMENT",
                "authId STRING UNIQUE ON CONFLICT ROLLBACK NOT NULL",
                "name STRING NOT NULL",
                "blocked BOOLEAN NOT NULL DEFAULT (0)",
            };
        }
        private static string[] GetSessionDefinition()
        {
            return new string[]
            {
                "id INTEGER PRIMARY KEY AUTOINCREMENT",
                "user STRING REFERENCES Users (username) ON DELETE CASCADE ON UPDATE CASCADE NOT NULL",
                "timeStarted TEXT NOT NULL DEFAULT ('01.01.2001 0:00:00')",
                "timeFinished TEXT NOT NULL DEFAULT ('01.01.2001 0:00:00')",
                "computer STRING REFERENCES Computers (authId) NOT NULL",
                "active BOOLEAN NOT NULL DEFAULT (0)"
            };
        }
        private static string[] GetTaskDefinition()
        {
            return new string[]
            {
                "id INTEGER PRIMARY KEY AUTOINCREMENT",
                "name STRING NOT NULL",
                "text TEXT",
                "passed BOOLEAN NOT NULL DEFAULT (0)",
                "dateStarted TEXT NOT NULL DEFAULT ('01.01.2001 0:00:00')",
                "dateFinished TEXT NOT NULL DEFAULT ('01.01.2001 0:00:00')",
                "hidden BOOLEAN NOT NULL DEFAULT (0)"
            };
        }
        private static string[] GetSubtaskDefinition()
        {
            return new string[]
            {
                "id INTEGER PRIMARY KEY AUTOINCREMENT",
                "task INT REFERENCES Tasks (id) ON DELETE CASCADE ON UPDATE CASCADE",
                "text TEXT NOT NULL",
                "passed BOOLEAN NOT NULL DEFAULT (0)",
                "performer STRING REFERENCES Users (username) NOT NULL"
            };
        }
        private static string[] GetEnumerationDefinition()
        {
            return new string[]
            {
                "id INTEGER PRIMARY KEY AUTOINCREMENT",
                "name STRING UNIQUE ON CONFLICT ROLLBACK NOT NULL",
                "content STRING NOT NULL",
                "hidden BOOLEAN NOT NULL DEFAULT (0)",
            };
        }
        private static string[] GetTemplateDefinition()
        {
            return new string[]
            {
                "id INTEGER PRIMARY KEY AUTOINCREMENT",
                "name STRING NOT NULL UNIQUE",
                "path STRING NOT NULL",
                "suggestedPath STRING",
                "type STRING NOT NULL",
                "hidden BOOLEAN NOT NULL DEFAULT (0)",
                "parent INTEGER REFERENCES Templates (id) ON DELETE SET NULL ON UPDATE CASCADE",
            };
        }
        private static string[] GetTagDefinition()
        {
            return new string[]
            {
                "id INTEGER PRIMARY KEY AUTOINCREMENT",
                "template INTEGER REFERENCES Templates (id) ON DELETE CASCADE ON UPDATE CASCADE NOT NULL",
                "name STRING NOT NULL",
                "type STRING NOT NULL",
                "value STRING",
                "constant STRING REFERENCES Constants (id)",
                "enumeration STRING REFERENCES Enumerations (id) ON DELETE CASCADE",
                "parent INTEGER REFERENCES Tags (id) ON DELETE CASCADE ON UPDATE CASCADE",
                "hidden BOOLEAN NOT NULL DEFAULT (0)",
                "batch STRING"
            };
        }
        private static string[] GetConstantDefinition()
        {
            return new string[]
            {
                "id INTEGER PRIMARY KEY AUTOINCREMENT",
                "name STRING UNIQUE NOT NULL",
                "content STRING NOT NULL",
                "category STRING",
                "hidden BOOLEAN NOT NULL DEFAULT (0)",
            };
        }
    }
}

