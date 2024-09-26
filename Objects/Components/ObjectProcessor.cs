using Incas.Core.Classes;
using Incas.Objects.Models;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;

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
            if (cl == null)
            {
                return "";
            }
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
                request.Append($"[{f.Id}] TEXT,\n");
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
                classFields.Add(f.Id.ToString());
            }
            List<string> missingFields = classFields.Except(mapFields).ToList(); // отсутствующие поля
            List<string> excessFields = mapFields.Except(classFields).ToList(); // лишние поля
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
        public static void WriteObjects(Class cl, List<Object> objects)
        {
            string path = GetPathToObjectsMap(cl);
            Query q = new(ObjectProcessor.TableName, path);
            q.BeginTransaction();
            foreach (Object obj in objects)
            {
                ObjectProcessor.GetRequestForWritingObject(q, obj);
            }
            q.EndTransaction();
            q.ExecuteVoid();
        }
        public static void WriteObjects(Class cl, Object obj)
        {
            List<Object> objects = new();
            objects.Add(obj);
            ObjectProcessor.WriteObjects(cl, objects);
        }
        private static void GetRequestForWritingObject(Query q, Object obj)
        {
            Dictionary<string, string> values = new();
            foreach (FieldData fd in obj.Fields)
            {               
                values.Add(fd.ClassFieldId.ToString(), fd.Value);
            }
            if (obj.Id == Guid.Empty) // if NEW
            {              
                obj.Id = Guid.NewGuid();
                obj.AuthorId = ProgramState.CurrentUser.id;
                obj.CreationDate = DateTime.Now;
                values.Add(IdField, obj.Id.ToString());
                values.Add(AuthorField, obj.AuthorId.ToString());
                values.Add(DateCreatedField, obj.CreationDate.ToString());
                obj.AuthorId = ProgramState.CurrentUser.id;
                obj.CreationDate = DateTime.Now;
                
                q.InsertWithGuids(values);              
                q.SeparateCommand();
            }
            else // if EDIT
            {
                
                q.Update(values);
                
                q.WhereEqual(IdField, obj.Id.ToString());
                q.SeparateCommand();
            }
        }
        public static DataTable GetObjectsList(Class cl)
        {
            Query q = new("", GetPathToObjectsMap(cl));
            List<Objects.Models.Field> fields = cl.GetClassData().fields;
            List<string> fieldsRequest = new();
            fieldsRequest.Add($"[{IdField}]");
            fieldsRequest.Add($"[{DateCreatedField}] AS [Дата создания]");
            foreach (Models.Field f in fields)
            {
                fieldsRequest.Add($"[{f.Id}] AS [{f.VisibleName}]");
            }
            q.AddCustomRequest("SELECT " + string.Join(", ", fieldsRequest.ToArray()));
            q.AddCustomRequest($"FROM [{TableName}]");
            return q.Execute();
        }
        public static Object GetObject(Class cl, Guid id)
        {
            Object obj = new();
            obj.Fields = new();
            Query q = new(ObjectProcessor.TableName, GetPathToObjectsMap(cl));
            DataRow dr = q.Select().WhereEqual(IdField, id.ToString()).ExecuteOne();
            if (dr != null)
            {
                obj.Id = id;
                obj.AuthorId = Guid.Parse(dr[ObjectProcessor.AuthorField].ToString());
                obj.CreationDate = DateTime.Parse(dr[ObjectProcessor.DateCreatedField].ToString());
                obj.Comment = dr[ObjectProcessor.CommentField].ToString();
                foreach (Models.Field f in cl.GetClassData().fields)
                {
                    FieldData fd = new()
                    {
                        ClassFieldId = f.Id,
                        Value = dr[f.Id.ToString()].ToString()
                    };
                    obj.Fields.Add(fd);
                }
            }
            return obj;
        }
    }
}
