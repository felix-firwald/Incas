using DocumentFormat.OpenXml.Office2010.Excel;
using Incas.Core.Classes;
using Incas.Objects.Models;
using Incas.Objects.Views.Windows;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Incas.Objects.Components
{
    public static class ObjectProcessor
    {
        #region Tables
        /// <summary>
        /// Base table in .objinc, all types of classes must have this table
        /// </summary>
        public const string MainTable = "OBJECTS_MAP";

        /// <summary>
        /// Table in .objinc storing all edits of objects, typical only for documents
        /// </summary>
        public const string EditsTable = "EDITS_MAP";

        /// <summary>
        /// Table in .objinc storing user comments for documents storing in <see cref="MainTable"/>, typical only for documents
        /// </summary>
        public const string CommentsTable = "COMMENTS_MAP";
        #endregion
        #region Fields
        /// <summary>
        /// Service guid for <see cref="MainTable"/>, 
        /// <see cref="EditsTable"/>, 
        /// <see cref="AttachmentsTable"/>, 
        /// <see cref="CommentsTable"/>, all objects of all types of classes must have it
        /// </summary>
        public const string IdField = "SERVICE_ID";

        /// <summary>
        /// Service name for <see cref="MainTable"/>, all objects of all types of classes must have it
        /// </summary>
        public const string NameField = "OBJECT_NAME";

        /// <summary>
        /// Date created for <see cref="MainTable"/>, 
        /// <see cref="EditsTable"/>, 
        /// <see cref="AttachmentsTable"/>, 
        /// <see cref="CommentsTable"/>, typical only for docs
        /// </summary>
        public const string DateCreatedField = "CREATED_DATE";

        /// <summary>
        /// The user (guid) who created the object, typical for focs and models, but not for generators
        /// </summary>
        public const string AuthorField = "AUTHOR_ID";

        /// <summary>
        /// Custom status metafield (stored in bytes), for <see cref="MainTable"/>, default value is 0
        /// </summary>
        public const string StatusField = "STATUS_ID";

        /// <summary>
        /// Now is useless
        /// </summary>
        public const string MetaField = "META_INFORMATION";

        /// <summary>
        /// Date when the document is marked by user as <see cref="TerminatedField"/>
        /// </summary>
        public const string DateTerminatedField = "TERMINATED_DATE";

        /// <summary>
        /// Service status of document, default values is 0 (means document is still in working progress), 1 means document is marked by user as completed
        /// </summary>
        public const string TerminatedField = "TERMINATED";

        /// <summary>
        /// A link to source document that the edit or generator belongs to, for <see cref="EditsTable"/>
        /// </summary>
        public const string TargetObjectField = "TARGET_OBJECT_ID";

        /// <summary>
        /// A reference to source class of document that the generator belongs to, for <see cref="MainTable"/>
        /// </summary>
        public const string TargetClassField = "TARGET_CLASS_ID";

        /// <summary>
        /// Type of comment, for <see cref="CommentsTable"/>
        /// </summary>
        public const string TypeField = "TYPE";

        /// <summary>
        /// Data, for <see cref="EditsTable"/> and <see cref="CommentsTable"/>
        /// </summary>
        public const string DataField = "DATA";

        #endregion
        /// <summary>
        /// Get path to .objinc file by id of class
        /// </summary>
        /// <returns>Full path to a database</returns>
        public static string GetPathToObjectsMap(Class cl)
        {
            return cl == null ? "" : $"{ProgramState.ObjectsPath}\\{cl.identifier}.objinc";
        }

        /// <summary>
        /// Get path to .objinc file by guid
        /// </summary>
        /// <returns>Full path to a database</returns>
        private static string GetPathToObjectsMap(Guid id)
        {
            return $"{ProgramState.ObjectsPath}\\{id}.objinc";
        }
        /// <summary>
        /// Get path to folder named by guid of class where placed an attached files
        /// </summary>
        /// <returns></returns>
        public static string GetPathToAttachmentsFolder(Guid classId, Guid objectId)
        {
            string result = $"{ProgramState.ObjectsPath}\\{classId}\\{objectId}\\";
            Directory.CreateDirectory(result);
            return result;
        }
        /// <summary>
        /// Cleans up the Root/Objects folder if it contains useless files and folders
        /// </summary>
        public static void CleanGarbage()
        {
            using (Class cl = new())
            {
                DataTable dt = cl.GetAllClassesGuids();
                List<string> ids = new();
                foreach (DataRow dr in dt.Rows)
                {
                    ids.Add(dr[nameof(cl.identifier)].ToString());
                }
                foreach (string file in Directory.GetFiles(ProgramState.ObjectsPath))
                {
                    if (!ids.Contains(Path.GetFileNameWithoutExtension(file)))
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch { }
                    }
                }
                foreach (string dir in Directory.GetDirectories(ProgramState.ObjectsPath))
                {
                    if (!ids.Contains(new DirectoryInfo(dir).Name))
                    {
                        try
                        {
                            Directory.Delete(dir, true);
                        }
                        catch { }
                    }
                }
            }
        }

        /// <summary>
        /// Initializes .objinc file
        /// </summary>
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
                adding += $"CREATE TABLE [{EditsTable}] ([{IdField}] TEXT UNIQUE, [{TargetObjectField}] TEXT, [{StatusField}] TEXT, [{DateCreatedField}] TEXT, [{AuthorField}] TEXT, [{DataField}] TEXT);\n";
                adding += $"CREATE TABLE [{CommentsTable}] ([{IdField}] TEXT UNIQUE, [{TargetObjectField}] TEXT, [{TypeField}] TEXT, [{DateCreatedField}] TEXT, [{AuthorField}] TEXT, [{DataField}] TEXT);";
            }
            else if (data.ClassType == ClassType.Generator)
            {
                request.Append($"[{TargetClassField}] TEXT, [{TargetObjectField}] TEXT, ");
            }
            List<string> customFields = [];
            foreach (Incas.Objects.Models.Field f in cl.GetClassData().GetSavebleFields())
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

        /// <summary>
        /// Removes .objinc file
        /// </summary>
        /// <param name="cl"></param>
        public async static void DropObjectMap(Class cl)
        {
            await Task.Run(() =>
            {
                string path = GetPathToObjectsMap(cl);
                Query q = new("", path);
                q.BeginTransaction();
                q.AddCustomRequest($"DROP TABLE IF EXISTS [{MainTable}]; " +
                    $"DROP TABLE IF EXISTS [{EditsTable}]; " +
                    $"DROP TABLE IF EXISTS [{CommentsTable}]; ");
                q.EndTransaction();
                q.ExecuteVoid();
                Task.Delay(1000);
                try
                {
                    File.Delete(path);
                }
                catch { }
                try
                {
                    string folder = $"{ProgramState.ObjectsPath}\\{cl.identifier}";
                    Directory.Delete(folder, true);
                }
                catch { }
            });
        }
        /// <summary>
        /// Updates objects map [<see cref="MainTable"/>] in .objinc by a class
        /// </summary>
        /// <param name="cl"></param>
        public static void UpdateObjectMap(Class cl)
        {
            List<string> serviceColumns =
            [
                IdField,
                NameField,
                DateCreatedField,
                AuthorField,
                StatusField,
                DateTerminatedField,
                TargetObjectField,
                TargetClassField,
                TerminatedField
            ];
            string path = GetPathToObjectsMap(cl);
            Query q = new("", path);
            q.AddCustomRequest($"PRAGMA table_info([{MainTable}])");
            List<string> mapFields = [];
            DataTable dt = q.Execute();
            foreach (DataRow row in dt.Rows)
            {
                string name = row["name"].ToString();
                if (!serviceColumns.Contains(name))
                {
                    mapFields.Add(name);
                }
            }
            List<string> classFields = [];
            foreach (Models.Field f in cl.GetClassData().GetSavebleFields())
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
            ObjectProcessor.CheckComplianceWithConstraints(cl);
        }
        /// <summary>
        /// For checking compliance
        /// </summary>
        /// <returns>false if check is failed</returns>
        delegate bool Check(string value, Models.Field field);

        /// <summary>
        /// Check objects map after an update
        /// </summary>
        /// <param name="cl"></param>
        private async static void CheckComplianceWithConstraints(Class cl)
        {
            static List<Guid> CheckRows(Check func, Models.Field f, DataRowCollection collection)
            {
                List<Guid> errors = new();
                foreach (DataRow row in collection)
                {
                    if (!func(row[f.Id.ToString()].ToString(), f))
                    {
                        errors.Add(Guid.Parse(row[ObjectProcessor.IdField].ToString()));
                    }
                }
                return errors;
            }
            static List<Guid> CheckRowsEnumeration(Models.Field f, DataRowCollection collection)
            {
                List<Guid> errors = new();
                List<string> values = new();
                if (f.Type == FieldType.GlobalEnumeration)
                {
                    values = ProgramState.GetEnumeration(Guid.Parse(f.Value));
                    foreach (DataRow row in collection)
                    {
                        if (!CheckEnumeration(row[f.Id.ToString()].ToString(), f, values))
                        {
                            errors.Add(Guid.Parse(row[ObjectProcessor.IdField].ToString()));
                        }
                    }
                }
                else
                {
                    values = JsonConvert.DeserializeObject<List<string>>(f.Value);
                    foreach (DataRow row in collection)
                    {
                        if (!CheckEnumeration(row[f.Id.ToString()].ToString(), f, values))
                        {
                            errors.Add(Guid.Parse(row[ObjectProcessor.IdField].ToString()));
                        }
                    }
                }
                return errors;
            }
            static void ShowWindow(Models.Class cl, List<Guid> list, Models.Field field)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {                   
                    Objects.Views.Pages.ObjectsCorrector oc = new(cl, list, field);
                    DialogsManager.ShowPage(oc, "Исправление данных", "FIXING" + cl.identifier.ToString());
                });             
            }
            await Task.Run(() =>
            {              
                DataTable dt = new();
                Query q = new(ObjectProcessor.MainTable, GetPathToObjectsMap(cl));
                dt = q.Select().Execute();
                List<Models.Field> fields = cl.GetClassData().GetSavebleFields();
                foreach (Models.Field f in fields)
                {
                    ProgramStatusBar.SetText($"Проверка соответствия ограничениям поля [{f.Name}] для класса [{cl.name}]...");
                    List<Guid> guids;
                    switch (f.Type)
                    {
                        case FieldType.Variable:
                        case FieldType.Text:
                            if (f.NotNull == true)
                            {
                                guids = CheckRows(CheckText, f, dt.Rows);
                                if (guids.Count > 0)
                                {
                                    ShowWindow(cl, guids, f);
                                }                                
                            }                    
                            break;
                        case FieldType.Number:
                            guids = CheckRows(CheckNumber, f, dt.Rows);
                            if (guids.Count > 0)
                            {
                                ShowWindow(cl, guids, f);
                            }                              
                            break;
                        case FieldType.Date:
                            guids = CheckRows(CheckDate, f, dt.Rows);
                            if (guids.Count > 0)
                            {
                                ShowWindow(cl, guids, f);
                            }                               
                            break;
                        case FieldType.LocalEnumeration:
                            guids = CheckRowsEnumeration(f, dt.Rows);
                            if (guids.Count > 0)
                            {
                                ShowWindow(cl, guids, f);
                            }                                
                            break;
                        case FieldType.GlobalEnumeration:
                            guids = CheckRowsEnumeration(f, dt.Rows);
                            if (guids.Count > 0)
                            {
                                ShowWindow(cl, guids, f);
                            }                           
                            break;                      
                    }                   
                }
                ProgramStatusBar.Hide();
            });
        }
        /// <summary>
        /// Checks <see cref="FieldType.Text"/> and <see cref="FieldType.Variable"/>
        /// </summary>
        /// <returns>false if check is failed</returns>
        private static bool CheckText(string value, Models.Field field)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
        /// <summary>
        /// Checks <see cref="FieldType.Number"/>
        /// </summary>
        /// <returns>false if check is failed</returns>
        private static bool CheckNumber(string value, Models.Field field)
        {
            if (string.IsNullOrEmpty(value) && field.NotNull == false)
            {
                return true;
            }
            int num = 0;
            bool result = true;
            result = int.TryParse(value, out num);
            NumberFieldData nf = field.GetNumberFieldData();
            if (num < nf.MinValue || num > nf.MaxValue)
            {
                result = false;
            }
            return result;
        }
        /// <summary>
        /// Checks <see cref="FieldType.Date"/>
        /// </summary>
        /// <returns>false if check is failed</returns>
        private static bool CheckDate(string value, Models.Field field)
        {
            if (string.IsNullOrEmpty(value) && field.NotNull == false)
            {
                return true;
            }
            bool result = true;
            DateTime dt = new();
            result = DateTime.TryParse(value.ToString(), out dt);
            DateFieldData df = field.GetDateFieldData();
            if (dt < df.StartDate || dt > df.EndDate)
            {
                result = false;
            }
            return result;
        }
        /// <summary>
        /// Checks <see cref="FieldType.GlobalEnumeration"/> and <see cref="FieldType.LocalEnumeration"/>
        /// </summary>
        /// <returns>false if check is failed</returns>
        private static bool CheckEnumeration(string value, Models.Field field, List<string> values)
        {
            if (string.IsNullOrEmpty(value) && field.NotNull == false)
            {
                return true;
            }
            return values.Contains(value);
        }

        public async static Task<bool> IsUnique(Class cl, Models.Field field, string value)
        {
            bool result = true;
            await Task.Run(() =>
            {
                Query q = new(ObjectProcessor.MainTable, GetPathToObjectsMap(cl));
                DataRow dr = q.AddCustomRequest($"SELECT COUNT([{field.Id}]) AS COUNTER FROM [{MainTable}]")
                .WhereEqual(field.Id.ToString(), value)
                .ExecuteOne();
                if (int.Parse(dr["COUNTER"].ToString()) > 0)
                {
                    result = false;
                }
            });
            return result;
        }

        public async static Task<bool> WriteObjects(Class cl, List<Object> objects)
        {
            await Task.Run(() =>
            {
                ProgramStatusBar.SetText("Выполняется сохранение объектов...");
                string path = GetPathToObjectsMap(cl);
                Query q = new(ObjectProcessor.MainTable, path);
                q.BeginTransaction();
                foreach (Object obj in objects)
                {
                    ProgramStatusBar.SetText($"Выполняется сохранение объектов ({obj.Name})...");
                    ObjectProcessor.GetRequestForWritingObject(q, cl, cl.GetClassData(), obj);
                }
                q.EndTransaction();
                q.ExecuteVoid();
                ProgramStatusBar.Hide();              
            });
            return true;
        }
        public async static void WriteObjects(Class cl, Object obj)
        {
            List<Object> objects = [obj];
            await ObjectProcessor.WriteObjects(cl, objects);
        }
        private static void GetRequestForWritingObject(Query q, Class cl, ClassData data, Object obj)
        {
            Dictionary<string, string> values = new()
            {
                { NameField, obj.Name.ToString() },
                { StatusField, obj.Status.ToString() }
            };
            Dictionary<Guid, string> generators = new();
            foreach (FieldData fd in obj.Fields)
            {
                if (fd.ClassField.Type is not FieldType.GlobalConstant and not FieldType.LocalConstant)
                {
                    if (fd.ClassField.Type == FieldType.Generator)
                    {
                        generators.Add(Guid.Parse(fd.ClassField.Value), fd.Value);
                    }
                    else
                    {
                        values.Add(fd.ClassField.Id.ToString(), fd.Value);
                    }                  
                }
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
                    values.Add(TerminatedField, "0");
                }
                else if (data.ClassType == ClassType.Generator)
                {
                    values.Add(TargetClassField, obj.TargetClass.ToString());
                    values.Add(TargetObjectField, obj.TargetObject.ToString());
                }
                obj.AuthorId = ProgramState.CurrentUser.id;
                obj.CreationDate = DateTime.Now;

                q.Insert(values);
                q.SeparateCommand();
            }
            else // if EDIT
            {
                if (data.ClassType == ClassType.Document)
                {
                    values.Add(DateTerminatedField, obj.TerminatedDate.ToString());
                    values.Add(TerminatedField, obj.Terminated ? "1" : "0");
                }
                q.Update(values);
                q.WhereEqual(IdField, obj.Id.ToString());
                q.SeparateCommand();
            }
        }
        public static void SetObjectAsTerminated(Class cl, Object obj)
        {
            obj.TerminatedDate = DateTime.Now;
            obj.Terminated = true;
            ObjectProcessor.WriteObjects(cl, obj);
        }
        public static DataTable GetSimpleObjectsList(Class cl, string WhereCondition = null)
        {
            Query q = new("", GetPathToObjectsMap(cl));
            ClassData data = cl.GetClassData();
            List<Objects.Models.Field> fields = data.GetFieldsForMap();
            List<string> fieldsRequest = [$"[OBJECTS_MAP].[{IdField}]"];
            if (data.ClassType == ClassType.Document)
            {
                fieldsRequest.Add($"[OBJECTS_MAP].[{DateCreatedField}]");
            }
            else if (data.ClassType == ClassType.Generator)
            {
                fieldsRequest.Add($"[OBJECTS_MAP].[{TargetClassField}]");
                fieldsRequest.Add($"[OBJECTS_MAP].[{TargetObjectField}]");
            }
            fieldsRequest.Add($"[OBJECTS_MAP].[{NameField}]");
            foreach (Models.Field f in fields)
            {
                fieldsRequest.Add($"[{f.Id}] AS [{f.VisibleName}]");
            }
            q.AddCustomRequest("SELECT " + string.Join(", ", fieldsRequest.ToArray()));
            q.AddCustomRequest($"FROM [{MainTable}]");
            if (WhereCondition != null)
            {
                q.AddCustomRequest(WhereCondition);
            }
            return q.Execute();
        }
        #region Update For Correction
        public static DataTable GetSimpleObjectsWhereIdForCorrection(Class cl, List<Guid> list, Models.Field f)
        {
            Query q = new(ObjectProcessor.MainTable, GetPathToObjectsMap(cl));
            List<string> ids = new();
            foreach (Guid id in list)
            {
                ids.Add(id.ToString());
            }
            return q.Select($"[{IdField}], [{NameField}] AS [Наименование объекта], [{f.Id}] AS [Значение]").WhereIn(ObjectProcessor.IdField, ids).Execute();
        }
        public async static void UpdateFieldsByIdForCorrection(Class cl, Dictionary<string, string> fields, Models.Field f)
        {
            await Task.Run(() =>
            {
                Query q = new(ObjectProcessor.MainTable, GetPathToObjectsMap(cl));
                q.BeginTransaction();
                foreach (KeyValuePair<string, string> kvp in fields)
                {
                    Dictionary<string, string> dict = new();
                    dict.Add(f.Id.ToString(), kvp.Value);
                    q.Update(dict).WhereEqual(ObjectProcessor.IdField, kvp.Key.ToString());
                    q.SeparateCommand();
                }
                q.EndTransaction();
                q.ExecuteVoid();
            });           
        }
        #endregion
        public static DataTable GetObjectsList(Class cl, string WhereCondition = null)
        {
            Query q = new("", GetPathToObjectsMap(cl));
            ClassData data = cl.GetClassData();
            List<Objects.Models.Field> fields = data.GetFieldsForMap();
            List<string> fieldsRequest = [$"[OBJECTS_MAP].[{IdField}]"];
            fieldsRequest.Add($"[OBJECTS_MAP].[{StatusField}]");
            if (data.ClassType == ClassType.Document)
            {
                fieldsRequest.Add($"[OBJECTS_MAP].[{DateCreatedField}]");
            }
            else if (data.ClassType == ClassType.Generator)
            {
                fieldsRequest.Add($"[OBJECTS_MAP].[{TargetClassField}]");
                fieldsRequest.Add($"[OBJECTS_MAP].[{TargetObjectField}]");
            }
            fieldsRequest.Add($"[OBJECTS_MAP].[{NameField}]");
            List<string> innerJoins = [];
            foreach (Models.Field f in fields)
            {
                if (f.Type == FieldType.Relation)
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
            return q.Execute();
        }
        public static DataTable GetObjectsListWhereLike(Class cl, string field, string value)
        {
            string request = $"WHERE [{field}] LIKE '%{value}%'";
            return GetObjectsList(cl, request);
        }
        public static DataTable GetSimpleObjectsListWhereLike(Class cl, string field, string value)
        {
            string request = $"WHERE [{field}] LIKE '%{value}%'";
            return GetSimpleObjectsList(cl, request);
        }
        public static DataTable GetObjectsListWhereEqual(Class cl, string field, string value)
        {
            string request = $"WHERE [{field}] = '{value}'";
            return GetObjectsList(cl, request);
        }
        public static DataTable GetSimpleObjectsListWhereEqual(Class cl, string field, string value)
        {
            string request = $"WHERE [{field}] = '{value}'";
            return GetSimpleObjectsList(cl, request);
        }
        public static Object GetObject(Class cl, Guid id)
        {
            Object obj = new()
            {
                Fields = []
            };
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
                        DateTime cd;
                        DateTime.TryParse(dr[ObjectProcessor.DateCreatedField].ToString(), out cd);
                        obj.CreationDate = cd;
                    }
                    obj.Terminated = dr[ObjectProcessor.TerminatedField].ToString() != "0";
                    DateTime dt;
                    DateTime.TryParse(dr[ObjectProcessor.DateTerminatedField].ToString(), out dt);
                    obj.TerminatedDate = dt;
                }
                byte status = 0;
                byte.TryParse(dr[ObjectProcessor.StatusField].ToString(), out status);
                obj.Status = status;
                obj.Name = dr[ObjectProcessor.NameField].ToString();
                foreach (Models.Field f in cl.GetClassData().GetSavebleFields())
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
        public async static Task<List<Object>> GetObjects(Class cl, List<Guid> ids)
        {
            List<Object> obj = new();
            await Task.Run(() =>
            {
                ProgramStatusBar.SetText("Загрузка объектов...");
                foreach (Guid id in ids)
                {
                    ProgramStatusBar.SetText($"Загрузка объектов ({id})...");
                    obj.Add(GetObject(cl, id));
                }
                ProgramStatusBar.SetText($"Загрузка формы...");
                ProgramStatusBar.Hide();
            });
            return obj;
        }
        /// <summary>
        /// Only for generators, 
        /// 1 class is the generator objects map,
        /// 2 class is the target relation class of parent object
        /// </summary>
        /// <returns></returns>
        public static List<Object> GetRelatedObjects(Class source, Class parentClass, Guid parentObject)
        {
            List<Object> objs = new();
            List<Models.Field> Fields = source.GetClassData().GetSavebleFields();

            Query q = new(ObjectProcessor.MainTable, GetPathToObjectsMap(source));
            DataTable dt = q.Select()
                .WhereEqual(TargetClassField, parentClass.identifier.ToString())
                .WhereEqual(TargetObjectField, parentObject.ToString()).Execute();
            foreach (DataRow dr in dt.Rows)
            {
                Object obj = new();
                obj.Fields = new();
                if (dr != null)
                {
                    obj.Id = Guid.Parse(dr[ObjectProcessor.IdField].ToString());
                    obj.Name = dr[ObjectProcessor.NameField].ToString();
                    foreach (Models.Field f in Fields)
                    {
                        FieldData fd = new()
                        {
                            ClassField = f,
                            Value = dr[f.Id.ToString()].ToString()
                        };
                        obj.Fields.Add(fd);
                    }
                    objs.Add(obj);
                }
            }
            
            return objs;
        }
        public static void RemoveObject(Class cl, Guid id)
        {
            Query q = new(ObjectProcessor.MainTable, GetPathToObjectsMap(cl));
            q.Delete();
            q.WhereEqual(IdField, id.ToString()).Execute();
        }
        #region Attachments & Comments
        public static void WriteComment(Class cl, Object target, ObjectComment comment)
        {
            comment.CreationDate = DateTime.Now;
            comment.Type = CommentType.File;
            comment.AuthorId = ProgramState.CurrentUser.id;
            comment.TargetObject = target.Id;
            Dictionary<string, string> values = new()
            {
                { IdField, comment.Id.ToString() },
                { DateCreatedField, comment.CreationDate.ToString() },
                { AuthorField, comment.AuthorId.ToString() },
                { TypeField, comment.Type.ToString() },
                { TargetObjectField, comment.TargetObject.ToString() },
                { DataField, comment.Data }
            };

            Query q = new(ObjectProcessor.CommentsTable, GetPathToObjectsMap(cl));
            if (comment.Id == Guid.Empty) // if NEW
            {
                comment.Id = Guid.NewGuid();
                values[IdField] = comment.Id.ToString();
                q.Insert(values);
                q.ExecuteVoid();
            }
            else // if EDIT
            {
                values.Remove(IdField);
                q.Update(values)
                    .WhereEqual(IdField, comment.Id.ToString())
                    .ExecuteVoid();
            }
        }
        public static async Task<List<ObjectComment>> GetObjectComments(Class cl, Object target)
        {
            List<ObjectComment> result = new();
            await Task.Run(() =>
            {
                Query q = new(CommentsTable, GetPathToObjectsMap(cl));
                DataTable dt = q.Select()
                    .WhereEqual(TargetObjectField, target.Id.ToString())
                    .Execute();
                foreach (DataRow row in dt.Rows)
                {
                    ObjectComment oc = new()
                    {
                        Id = Guid.Parse(row[IdField].ToString()),
                        Class = cl.identifier,
                        CreationDate = DateTime.Parse(row[DateCreatedField].ToString()),
                        TargetObject = target.Id,
                        AuthorId = Guid.Parse(row[AuthorField].ToString()),
                        Data = row[DataField].ToString()
                    };
                    result.Add(oc);
                }
            });           
            return result;
        }
        public static void RemoveObjectComment(Class cl, ObjectComment comment)
        {
            Query q = new(CommentsTable, GetPathToObjectsMap(cl));
            q.Delete().WhereEqual(IdField, comment.Id.ToString()).ExecuteVoid();
        }
        #endregion
    }
}
