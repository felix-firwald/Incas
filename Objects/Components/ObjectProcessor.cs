using Incas.Core.Classes;
using Incas.Objects.Models;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Incas.Objects.Components
{
    public static class ObjectProcessor
    {
        public const string TableName = "OBJECTS_MAP";
        public const string IdField = "SERVICE_ID";
        public const string DateCreatedField = "CREATED_DATE";
        public const string AuthorField = "AUTHOR_ID";
        public const string CommentField = "COMMENT";
        public const string MetaField = "META_INFORMATION";
        public static string GetPathToObjectsMap(Class cl)
        {
            return $"{ProgramState.ObjectsPath}\\{cl.identifier}.objinc";
        }
        public static void InitializeObjectMap(Class cl)
        {
            string path = GetPathToObjectsMap(cl);
            Query q = new("", path);

            StringBuilder request = new($"CREATE TABLE [{TableName}] (\n");
            request.Append($" [{IdField}] TEXT UNIQUE, [{DateCreatedField}] TEXT, [{AuthorField}] TEXT, ");
            foreach (Incas.Objects.Models.Field f in cl.GetClassData().fields)
            {
                request.Append($"[{f.Guid}] TEXT,\n");
            }
            request.Append($"[{CommentField}] TEXT, [{MetaField}] TEXT\n)");
            q.AddCustomRequest(request.ToString());
            q.ExecuteVoid();
        }
        public static void DropObjectMap(Class cl)
        {
            string path = GetPathToObjectsMap(cl);
            File.Delete(path);
        }
        public static void UpdateObjectMap(Class cl)
        {
            List<string> serviceColumns = new()
            {
                IdField,
                DateCreatedField,
                AuthorField,
                CommentField,
                MetaField,
            };
            string path = GetPathToObjectsMap(cl);
            Query q = new("", path);
            q.AddCustomRequest($"PRAGMA table_info([{TableName}])");
            List<string> mapFields = new();
            DataTable dt = q.Execute();
            foreach (DataRow row in dt.Rows)
            {
                string name = row["name"].ToString();
                if (!serviceColumns.Contains(name))
                {
                    mapFields.Add(name);
                }
            }
            List<string> classFields = new();
            foreach (Models.Field f in cl.GetClassData().fields)
            {
                classFields.Add(f.Guid.ToString());
            }
            List<string> missingFields = (List<string>)classFields.Except(mapFields); // отсутствующие поля
            List<string> excessFields = (List<string>)mapFields.Except(classFields); // лишние поля
            StringBuilder updateRequest = new("BEGIN TRANSACTION;");
            foreach (string misf in missingFields)
            {
                updateRequest.Append($"ALTER TABLE [{TableName}] ADD COLUMN [{misf}] TEXT;");
            }
            foreach (string excf in excessFields)
            {
                updateRequest.Append($"ALTER TABLE [{TableName}] DROP COLUMN [{excf}];");
            }
            updateRequest.Append("COMMIT");
            q.Clear();
            q.AddCustomRequest(updateRequest.ToString());
            q.ExecuteVoid();
        }
        public static Guid WriteNewObject(Object obj)
        {
            obj.Id = Guid.NewGuid();
            obj.AuthorId = ProgramState.CurrentUser.id;
            obj.CreationDate = DateTime.Now;
            return obj.Id;
        }
    }
}
