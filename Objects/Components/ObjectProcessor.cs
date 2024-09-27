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
        #region Tables
        public const string MainTable = "OBJECTS_MAP";
        public const string EditsTable = "EDITS_MAP";
        public const string AttachmentsTable = "ATTACHMENTS_MAP";
        public const string CommentsTable = "COMMENTS_MAP";
        #endregion
        #region Fields
        public const string IdField = "SERVICE_ID";
        public const string NameField = "OBJECT_NAME";
        public const string DateCreatedField = "CREATED_DATE";
        public const string AuthorField = "AUTHOR_ID";
        public const string StatusField = "STATUS_ID";
        public const string MetaField = "META_INFORMATION";
        #endregion
        public static string GetPathToObjectsMap(Class cl)
        {
            if (cl == null)
            {
                return "";
            }
            return $"{ProgramState.ObjectsPath}\\{cl.identifier}.objinc";
        }
        private static string GetPathToObjectsMap(Guid id)
        {
            return $"{ProgramState.ObjectsPath}\\{id}.objinc";
        }

        public static void InitializeObjectMap(Class cl)
        {
            ClassData data = cl.GetClassData();
            string path = GetPathToObjectsMap(cl);
            Query q = new("", path);

            StringBuilder request = new($"CREATE TABLE [{MainTable}] (\n");
            request.Append($" [{IdField}] TEXT UNIQUE, [{NameField}] TEXT, [{AuthorField}] TEXT, ");
            if (data.ClassType == ClassType.Document)
            {
                request.Append($"[{DateCreatedField}] TEXT, [{StatusField}] TEXT, ");
            }
            foreach (Incas.Objects.Models.Field f in cl.GetClassData().fields)
            {
                request.Append($"[{f.Id}] TEXT,\n");
            }
            request.Append($"[{MetaField}] TEXT\n)");
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
                NameField,
                DateCreatedField,
                AuthorField,
                StatusField,
                MetaField,
            };
            string path = GetPathToObjectsMap(cl);
            Query q = new("", path);
            q.AddCustomRequest($"PRAGMA table_info([{MainTable}])");
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
                updateRequest.Append($"ALTER TABLE [{MainTable}] ADD COLUMN [{misf}] TEXT;");
            }
            foreach (string excf in excessFields)
            {
                updateRequest.Append($"ALTER TABLE [{MainTable}] DROP COLUMN [{excf}];");
            }
            updateRequest.Append("COMMIT");
            q.Clear();
            q.AddCustomRequest(updateRequest.ToString());
            q.ExecuteVoid();
        }
        public static void WriteObjects(Class cl, List<Object> objects)
        {
            string path = GetPathToObjectsMap(cl);
            Query q = new(ObjectProcessor.MainTable, path);
            q.BeginTransaction();
            foreach (Object obj in objects)
            {
                ObjectProcessor.GetRequestForWritingObject(q, cl.GetClassData(), obj);
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
        private static void GetRequestForWritingObject(Query q, ClassData data, Object obj)
        {
            Dictionary<string, string> values = new();
            values.Add(NameField, obj.Name.ToString());
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
                if (data.ClassType == ClassType.Document)
                {
                    values.Add(DateCreatedField, obj.CreationDate.ToString());
                    values.Add(StatusField, obj.Status.ToString());
                }
                obj.AuthorId = ProgramState.CurrentUser.id;
                obj.CreationDate = DateTime.Now;
                
                q.Insert(values);              
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
            ClassData data = cl.GetClassData();
            List<Objects.Models.Field> fields = data.GetFieldsForMap();
            List<string> fieldsRequest = new();
            fieldsRequest.Add($"[OBJECTS_MAP].[{IdField}]");
            if (data.ClassType == ClassType.Document)
            {
                fieldsRequest.Add($"[OBJECTS_MAP].[{DateCreatedField}]");
                fieldsRequest.Add($"[OBJECTS_MAP].[{StatusField}]");
            }           
            fieldsRequest.Add($"[OBJECTS_MAP].[{NameField}]");
            List<string> innerJoins = new();
            foreach (Models.Field f in fields)
            {
                if (f.Type == Templates.Components.TagType.Relation)
                {       
                    BindingData bd = f.GetBindingData();
                    string dbName = bd.Class.ToString("N");
                    q.AttachDatabase(GetPathToObjectsMap(bd.Class), dbName);
                    
                    char[] charArray = dbName.ToCharArray();
                    Array.Reverse(charArray);
                    string dbNamePseudoname = new(charArray);
                    fieldsRequest.Add($"[{bd.Field}] AS [{f.VisibleName}]");
                    innerJoins.Add($"LEFT JOIN \"{dbName}\".[{MainTable}] AS [{dbNamePseudoname}] ON [{dbNamePseudoname}].[{IdField}] = [{MainTable}].\"{f.Id}\"");
                }
                else
                {
                    fieldsRequest.Add($"[{f.Id}] AS [{f.VisibleName}]");
                }          
            }
            q.AddCustomRequest("SELECT " + string.Join(", ", fieldsRequest.ToArray()));
            q.AddCustomRequest($"FROM [{MainTable}]");
            q.AddCustomRequest(string.Join("\n", innerJoins));
            return q.Execute();
        }
        public static Object GetObject(Class cl, Guid id)
        {
            Object obj = new();
            obj.Fields = new();
            Query q = new(ObjectProcessor.MainTable, GetPathToObjectsMap(cl));
            DataRow dr = q.Select().WhereEqual(IdField, id.ToString()).ExecuteOne();
            if (dr != null)
            {
                obj.Id = id;
                obj.AuthorId = Guid.Parse(dr[ObjectProcessor.AuthorField].ToString());
                if (cl.GetClassData().ClassType == ClassType.Document)
                {
                    if (dr[ObjectProcessor.DateCreatedField] is not null)
                    {
                        obj.CreationDate = DateTime.Parse(dr[ObjectProcessor.DateCreatedField].ToString());
                    }
                    obj.Status = Guid.Parse(dr[ObjectProcessor.StatusField].ToString());
                }
                
                obj.Name = dr[ObjectProcessor.NameField].ToString();
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
        public static void RemoveObject(Class cl, Guid id)
        {
            Query q = new(ObjectProcessor.MainTable, GetPathToObjectsMap(cl));
            q.Delete();
            q.WhereEqual(IdField, id.ToString()).Execute();
        }
    }
}
