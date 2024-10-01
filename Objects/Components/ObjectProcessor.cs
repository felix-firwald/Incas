using Incas.Core.Classes;
using Incas.Objects.Models;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using Spire.Pdf.Graphics;

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
        public const string DateCreatedField = "CREATED_DATE"; // only docs
        public const string AuthorField = "AUTHOR_ID";
        public const string StatusField = "STATUS_ID";
        public const string MetaField = "META_INFORMATION";
        public const string DateTerminatedField = "TERMINATED_DATE"; // only docs
        public const string TerminatedField = "TERMINATED"; // only docs
        public const string ObjectLinkField = "TARGET_OBJECT_ID"; // only docs
        public const string TypeField = "TYPE"; // only docs
        public const string DataField = "DATA"; // only docs
        #endregion
        public const int MaxObjectCompareCount = 5;
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
            string adding = "";
            Query q = new("", path);

            StringBuilder request = new($"BEGIN TRANSACTION; CREATE TABLE [{MainTable}] (\n");
            request.Append($" [{IdField}] TEXT UNIQUE, [{NameField}] TEXT, [{StatusField}] TEXT, [{AuthorField}] TEXT, ");
            if (data.ClassType == ClassType.Document)
            {
                request.Append($"[{DateCreatedField}] TEXT, [{DateTerminatedField}] TEXT, [{TerminatedField}] TEXT, ");
                adding += $"CREATE TABLE [{EditsTable}] ([{IdField}] TEXT UNIQUE, [{ObjectLinkField}] TEXT, [{StatusField}] TEXT, [{DateCreatedField}] TEXT, [{AuthorField}] TEXT, [{DataField}] TEXT);\n";
                adding += $"CREATE TABLE [{CommentsTable}] ([{IdField}] TEXT UNIQUE, [{ObjectLinkField}] TEXT, [{TypeField}] TEXT, [{DateCreatedField}] TEXT, [{AuthorField}] TEXT, [{DataField}] TEXT);";
            }
            List<string> customFields = new();
            foreach (Incas.Objects.Models.Field f in cl.GetClassData().fields)
            {
                customFields.Add($"[{f.Id}] TEXT");
            }
            request.Append(string.Join(", ", customFields));
            request.Append(");");
            request.Append(adding);
            request.Append("\nCOMMIT");
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
                DateTerminatedField,
                TerminatedField
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
            values.Add(StatusField, obj.Status.ToString());
            foreach (FieldData fd in obj.Fields)
            {               
                values.Add(fd.ClassField.Id.ToString(), fd.Value);
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
                }
                //values.Add(StatusField, obj.Status.ToString());
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
        public static DataTable GetObjectsList(Class cl, string WhereCondition = null)
        {
            Query q = new("", GetPathToObjectsMap(cl));
            ClassData data = cl.GetClassData();
            List<Objects.Models.Field> fields = data.GetFieldsForMap();
            List<string> fieldsRequest = new();
            fieldsRequest.Add($"[OBJECTS_MAP].[{IdField}]");
            if (data.ClassType == ClassType.Document)
            {
                fieldsRequest.Add($"[OBJECTS_MAP].[{DateCreatedField}]");
            }
            //fieldsRequest.Add($"[OBJECTS_MAP].[{StatusField}]");
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
            if (WhereCondition != null)
            {
                q.AddCustomRequest(WhereCondition);
            }
            if (data.ClassType == ClassType.Model)
            {
                q.OrderByASC("OBJECT_NAME");
            }
            return q.Execute();
        }
        public static DataTable GetObjectsListWhereLike(Class cl, string field, string value)
        {
            string request = $"WHERE [{field}] LIKE '%{value}%'";
            return GetObjectsList(cl, request);
        }
        public static DataTable GetObjectsListWhereEqual(Class cl, string field, string value)
        {
            string request = $"WHERE [{field}] = '{value}'";
            return GetObjectsList(cl, request);
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
                }
                byte status = 0;
                byte.TryParse(dr[ObjectProcessor.StatusField].ToString(), out status);
                obj.Status = status;
                obj.Name = dr[ObjectProcessor.NameField].ToString();
                foreach (Models.Field f in cl.GetClassData().fields)
                {
                    FieldData fd = new()
                    {
                        ClassField = f,
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
