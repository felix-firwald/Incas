using Incas.Core.Classes;
using Incas.Objects.Components;
using Incas.Objects.Documents.Components;
using Incas.Objects.Exceptions;
using Incas.Objects.Interfaces;
using Incas.Objects.Models;
using Incas.Objects.Processes.Components;
using Incas.Objects.StaticModels.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Incas.Objects.Engine
{
    public static class Processor
    {

        /// <summary>
        /// Get path to .objinc file by id of class
        /// </summary>
        /// <returns>Full path to a database</returns>
        public static string GetPathToObjectsMap(IClass cl)
        {
            return cl == null ? "" : $"{ProgramState.CurrentWorkspace.ObjectsPath}\\{cl.Id}.objinc";
        }

        /// <summary>
        /// Get path to .objinc file by guid
        /// </summary>
        /// <returns>Full path to a database</returns>
        private static string GetPathToObjectsMap(Guid id)
        {
            return $"{ProgramState.CurrentWorkspace.ObjectsPath}\\{id}.objinc";
        }
        /// <summary>
        /// Get path to folder named by guid of class where placed an attached files
        /// </summary>
        /// <returns></returns>
        public static string GetPathToAttachmentsFolder(Guid classId, Guid objectId)
        {
            string result = $"{ProgramState.CurrentWorkspace.ObjectsPath}\\{classId}\\{objectId}\\";
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
                    ids.Add(dr[nameof(cl.Id)].ToString());
                }
                foreach (string file in Directory.GetFiles(ProgramState.CurrentWorkspace.ObjectsPath))
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
                foreach (string dir in Directory.GetDirectories(ProgramState.CurrentWorkspace.ObjectsPath))
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
        /// Fixes .objinc file bugs
        /// </summary>
        /// <param name="cl"></param>
        public static void FixObjectMap(IClass cl)
        {
            ClassData data = cl.GetClassData();
            if (data.PresetsEnabled)
            {
                string path = GetPathToObjectsMap(cl);

            }
        }

        /// <summary>
        /// Initializes .objinc file
        /// </summary>
        public static void InitializeObjectMap(IClass cl)
        {
            ClassData data = cl.GetClassData();
            string path = GetPathToObjectsMap(cl);
            Query q = new("", path);
            q.OnSQLErrorDetected += OnSQLErrorDetected;
            q.BeginTransaction();
            IObject obj = Helpers.CreateObjectByType(cl);
            List<string> fields = new();
            fields.Add(Helpers.IdField);
            fields.Add(Helpers.NameField);
            foreach (KeyValuePair<string, string> pair in obj.AddServiceFields(new()))
            {
                fields.Add(pair.Key);
            }
            foreach (Models.Field f in data.GetSavebleFields())
            {
                fields.Add(f.Id.ToString());
            }
            q.CreateTable(Helpers.MainTable, fields);
            if (Helpers.IsEditsMapRequired(cl))
            {
                q.SeparateCommand();
                List<DbField> editsMapFields = new();
                q.CreateTable(Helpers.EditsTable, new List<string>() {
                    Helpers.IdField,
                    Helpers.TargetObjectField,
                    Helpers.StatusField,
                    Helpers.DateCreatedField,
                    Helpers.AuthorField,
                    Helpers.DataField});
            }
            if (Helpers.IsPresetsMapRequired(cl))
            {
                q.SeparateCommand();
                List<DbField> editsMapFields = new();
                q.CreateTable(Helpers.PresetsTable, new List<string>() {
                    Helpers.IdField,
                    Helpers.NameField,
                    Helpers.DateCreatedField,
                    Helpers.AuthorField,
                    Helpers.DataField});
            }
            q.SeparateCommand();
            q.EndTransaction();
            q.ExecuteVoid();
        }

        /// <summary>
        /// Removes .objinc file
        /// </summary>
        /// <param name="cl"></param>
        public async static void DropObjectMap(IClass cl)
        {
            await Task.Run(() =>
            {
                string path = GetPathToObjectsMap(cl);
                Query q = new("", path);
                q.OnSQLErrorDetected += OnSQLErrorDetected;
                q.BeginTransaction();
                q.AddCustomRequest($"DROP TABLE IF EXISTS [{Helpers.MainTable}]; " +
                    $"DROP TABLE IF EXISTS [{Helpers.EditsTable}]; " +
                    $"DROP TABLE IF EXISTS [{Helpers.CommentsTable}]; ");
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
                    string folder = $"{ProgramState.CurrentWorkspace.ObjectsPath}\\{cl.Id}";
                    Directory.Delete(folder, true);
                }
                catch { }
            });
        }
        /// <summary>
        /// Updates objects map [<see cref="MainTable"/>] in .objinc by a class
        /// </summary>
        /// <param name="cl"></param>
        public static void UpdateObjectMap(IClass cl)
        {
            List<string> serviceColumns =
            [
                Helpers.IdField,
                Helpers.NameField,
                Helpers.DateCreatedField,
                Helpers.AuthorField,
                Helpers.StatusField,
                Helpers.DateTerminatedField,
                Helpers.TargetObjectField,
                Helpers.TargetClassField,
                Helpers.TerminatedField,
                Helpers.DataField,
                Helpers.PresetField
            ];
            string path = GetPathToObjectsMap(cl);
            Query q = new("", path);
            q.OnSQLErrorDetected += OnSQLErrorDetected;
            q.AddCustomRequest($"PRAGMA table_info([{Helpers.MainTable}])");
            List<string> mapFields = []; // columns in map
            List<string> mapServiceFields = []; // columns in map
            DataTable dt = q.Execute();
            foreach (DataRow row in dt.Rows)
            {
                string name = row["name"].ToString();
                if (!serviceColumns.Contains(name)) // if column not in service columns
                {
                    mapFields.Add(name);
                }
                else
                {
                    mapServiceFields.Add(name);
                }
            }
            List<string> classFields = [];
            ClassData classData = cl.GetClassData();
            foreach (Models.Field f in classData.GetSavebleFields())
            {
                classFields.Add(f.Id.ToString());
            }
            List<string> missingFields = classFields.Except(mapFields).ToList(); // отсутствующие поля
            List<string> excessFields = mapFields.Except(classFields).ToList(); // лишние поля

            StringBuilder updateRequest = new("BEGIN TRANSACTION;");
            updateRequest.Append($"CREATE TABLE IF NOT EXISTS [{Helpers.PresetsTable}] ([{Helpers.IdField}] TEXT UNIQUE, [{Helpers.NameField}] TEXT, [{Helpers.DateCreatedField}] TEXT, [{Helpers.AuthorField}] TEXT, [{Helpers.DataField}] TEXT);");
            foreach (string misf in missingFields)
            {
                updateRequest.Append($"ALTER TABLE [{Helpers.MainTable}] ADD COLUMN [{misf}] TEXT;");
            }
            foreach (string excf in excessFields)
            {
                updateRequest.Append($"ALTER TABLE [{Helpers.MainTable}] DROP COLUMN [{excf}];");
            }
            if (cl.Type != ClassType.StaticModel && !mapServiceFields.Contains(Helpers.PresetField)) // if preset column must be placed but not found
            {
                updateRequest.Append($"ALTER TABLE [{Helpers.MainTable}] ADD COLUMN [{Helpers.PresetField}] TEXT;");
            }
            updateRequest.Append("COMMIT");
            q.Clear();
            q.AddCustomRequest(updateRequest.ToString());
            q.ExecuteVoid();
            CheckComplianceWithConstraints(cl);
        }
        /// <summary>
        /// For checking compliance
        /// </summary>
        /// <returns>false if check is failed</returns>
        delegate bool Check(string value, Models.Field field);

        #region Compliance
        /// <summary>
        /// Check objects map after an update
        /// </summary>
        /// <param name="cl"></param>
        private async static void CheckComplianceWithConstraints(IClass cl)
        {
            static List<Guid> CheckRows(Check func, Models.Field f, DataRowCollection collection)
            {
                List<Guid> errors = new();
                foreach (DataRow row in collection)
                {
                    if (!func(row[f.Id.ToString()].ToString(), f))
                    {
                        errors.Add(Guid.Parse(row[Helpers.IdField].ToString()));
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
                            errors.Add(Guid.Parse(row[Helpers.IdField].ToString()));
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
                            errors.Add(Guid.Parse(row[Helpers.IdField].ToString()));
                        }
                    }
                }
                return errors;
            }
            static void ShowWindow(IClass cl, List<Guid> list, Models.Field field)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Views.Pages.ObjectsCorrector oc = new(cl, list, field);
                    DialogsManager.ShowPageWithGroupBox(oc, $"Исправление данных ({field.Name})", "FIX" + field.Id.ToString());
                });
            }
            await Task.Run(() =>
            {
                DataTable dt = new();
                Query q = new(Helpers.MainTable, GetPathToObjectsMap(cl));
                dt = q.Select().Execute();
                List<Models.Field> fields = cl.GetClassData().GetSavebleFields();
                foreach (Models.Field f in fields)
                {
                    ProgramStatusBar.SetText($"Проверка соответствия ограничениям поля [{f.Name}] для класса [{cl.Name}]...");
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
        #endregion

        #region Presets
        public async static Task<bool> IsUnique(IClass cl, Models.Field field, string value)
        {
            bool result = true;
            await Task.Run(() =>
            {
                Query q = new(Helpers.MainTable, GetPathToObjectsMap(cl));
                q.OnSQLErrorDetected += OnSQLErrorDetected;
                DataRow dr = q.AddCustomRequest($"SELECT COUNT([{field.Id}]) AS COUNTER FROM [{Helpers.MainTable}]")
                .WhereEqual(field.Id.ToString(), value)
                .ExecuteOne();
                if (int.Parse(dr["COUNTER"].ToString()) > 0)
                {
                    result = false;
                }
            });
            return result;
        }
        public async static Task<bool> WritePreset(IClass cl, Preset preset)
        {
            await Task.Run(() =>
            {
                ProgramStatusBar.SetText("Выполняется сохранение пресета...");
                string path = GetPathToObjectsMap(cl);
                Query q = new(Helpers.PresetsTable, path);
                q.OnSQLErrorDetected += OnSQLErrorDetected;
                q.BeginTransaction();
                if (preset.Id == Guid.Empty)
                {
                    preset.Id = Guid.NewGuid();
                    preset.CreatedDate = DateTime.Now;
                    preset.AuthorId = ProgramState.CurrentWorkspace.CurrentUser.Id;
                    q.Insert(new()
                    {
                        { Helpers.IdField, preset.Id.ToString()},
                        { Helpers.NameField, preset.Name},
                        { Helpers.DateCreatedField, preset.CreatedDate.ToString()},
                        { Helpers.AuthorField, preset.AuthorId.ToString()},
                        { Helpers.DataField, preset.GetData()},
                    });
                    q.SeparateCommand();
                }
                else
                {
                    q.Update(new()
                    {
                        { Helpers.NameField, preset.Name},
                        { Helpers.DataField, preset.GetData()},
                    });
                    q.WhereEqual(Helpers.IdField, preset.Id.ToString());
                    q.SeparateCommand();
                    q.Update(Helpers.MainTable, preset.GetValues());
                    q.WhereEqual(Helpers.PresetField, preset.Id.ToString());
                    q.SeparateCommand();
                }
                q.EndTransaction();
                q.ExecuteVoid();
                ProgramStatusBar.Hide();
            });
            return true;
        }
        public async static Task<bool> ApplyPresetToRelevant(IClass cl, Preset preset)
        {
            await Task.Run(() =>
            {
                string path = GetPathToObjectsMap(cl);
                Query q = new(Helpers.MainTable, path);
                q.OnSQLErrorDetected += OnSQLErrorDetected;
                q.BeginTransaction();
                q.Update(Helpers.MainTable, new() { { Helpers.PresetField, preset.Id.ToString() } });
                q.Where(preset.GetValues());
                q.SeparateCommand();
                q.EndTransaction();
                q.ShowRequest();
                q.ExecuteVoid();
            });
            return true;
        }
        public async static Task<bool> RemovePreset(IClass cl, PresetReference preset)
        {
            await Task.Run(() =>
            {
                string path = GetPathToObjectsMap(cl);
                Query q = new(Helpers.PresetsTable, path);
                q.OnSQLErrorDetected += OnSQLErrorDetected;
                q.BeginTransaction();
                q.Delete();
                q.WhereEqual(Helpers.IdField, preset.Id.ToString());
                q.SeparateCommand();
                q.Update(Helpers.MainTable, new()
                    {
                        { Helpers.PresetField, Guid.Empty.ToString() }
                    });
                q.WhereEqual(Helpers.PresetField, preset.Id.ToString());
                q.SeparateCommand();
                q.EndTransaction();
                q.ExecuteVoid();
            });
            return true;
        }
        public static List<PresetReference> GetPresetsReferences(IClass cl)
        {
            List<PresetReference> presets = new();
            string path = GetPathToObjectsMap(cl);
            Query q = new(Helpers.PresetsTable, path);
            q.OnSQLErrorDetected += OnSQLErrorDetected;
            q.Select();
            DataTable dt = q.Execute();
            foreach (DataRow dr in dt.Rows)
            {
                PresetReference preset = new()
                {
                    Id = Guid.Parse(dr[Helpers.IdField].ToString()),
                    Name = dr[Helpers.NameField].ToString()
                };
                presets.Add(preset);
            }
            return presets;
        }
        public static Preset GetPreset(IClass cl, PresetReference ps)
        {
            string path = GetPathToObjectsMap(cl);
            Query q = new(Helpers.PresetsTable, path);
            q.OnSQLErrorDetected += OnSQLErrorDetected;
            DataRow dr = q.Select().WhereEqual(Helpers.IdField, ps.Id.ToString()).ExecuteOne();
            if (dr is null)
            {
                return null;
            }
            Preset preset = new()
            {
                Id = ps.Id,
                CreatedDate = DateTime.Parse(dr[Helpers.DateCreatedField].ToString()),
                AuthorId = Guid.Parse(dr[Helpers.AuthorField].ToString()),
                Name = dr[Helpers.NameField].ToString()
            };
            preset.SetData(dr[Helpers.DataField].ToString());
            return preset;
        }
        public static Preset GetPreset(IClass cl, Guid id)
        {
            PresetReference pr = new()
            {
                Id = id
            };
            return GetPreset(cl, pr);
        }
        public async static Task<bool> RemovePreset(IClass cl, Preset preset)
        {
            await Task.Run(() =>
            {
                ProgramStatusBar.SetText("Выполняется удаление пресета...");
                string path = GetPathToObjectsMap(cl);
                Query q = new(Helpers.PresetsTable, path);
                q.OnSQLErrorDetected += OnSQLErrorDetected;
                q.BeginTransaction();
                q.Delete();
                q.WhereEqual(Helpers.IdField, preset.Id.ToString());
                q.EndTransaction();
                q.ExecuteVoid();
                ProgramStatusBar.Hide();
            });
            return true;
        }
        #endregion

        public async static Task<bool> WriteObjects(IClass cl, List<IObject> objects)
        {
            await Task.Run(() =>
            {
                ProgramStatusBar.SetText("Выполняется сохранение объектов...");
                string path = GetPathToObjectsMap(cl);
                Query q = new(Helpers.MainTable, path);
                q.OnSQLErrorDetected += OnSQLErrorDetected;
                q.BeginTransaction();
                foreach (IObject obj in objects as List<IObject>)
                {
                    ProgramStatusBar.SetText($"Выполняется сохранение объектов ({obj.Name})...");
                    Processor.GetRequestForWritingObject(q, cl, cl.GetClassData(), obj);
                }
                q.EndTransaction();
                q.ExecuteVoid();
                ProgramStatusBar.Hide();
            });
            return true;
        }
        public async static void WriteObjects(IClass cl, IObject obj)
        {
            List<IObject> objects = [obj];
            await WriteObjects(cl, objects);
        }
        private static void GetRequestForWritingObject(Query q, IClass cl, ClassData data, IObject obj)
        {
            Dictionary<string, string> values = new();
            if (string.IsNullOrEmpty(obj.Name))
            {
                throw new NotNullFailed("Не присвоено имя сохраняемому объекту.");
            }
            values.Add(Helpers.NameField, obj.Name.ToString());
            foreach (FieldData fd in obj.Fields)
            {
                if (fd.ClassField.Type is not FieldType.GlobalConstant and not FieldType.LocalConstant)
                {
                    values.Add(fd.ClassField.Id.ToString(), fd.Value);
                }
            }
            if (obj.Id == Guid.Empty) // if NEW
            {
                obj.Id = Guid.NewGuid();
                obj.Initialize();                
                values.Add(Helpers.IdField, obj.Id.ToString());
                obj.AddServiceFields(values);                
                q.Insert(values);
                q.SeparateCommand();
            }
            else // if EDIT
            {
                obj.AddServiceFields(values);
                q.Update(values);
                q.WhereEqual(Helpers.IdField, obj.Id.ToString());
                q.SeparateCommand();
            }           
        }
        public static void SetObjectAsTerminated(IClass cl, ITerminable obj)
        {
            obj.TerminatedDate = DateTime.Now;
            obj.Terminated = true;
            WriteObjects(cl, (IObject)obj);
        }
        public static DataTable GetSimpleObjectsList(IClass cl, string WhereCondition = null)
        {
            Query q = new("", GetPathToObjectsMap(cl));
            q.OnSQLErrorDetected += OnSQLErrorDetected;
            ClassData data = cl.GetClassData();
            List<Models.Field> fields = data.GetFieldsForMap();
            List<string> fieldsRequest = [$"[OBJECTS_MAP].[{Helpers.IdField}]"];
            if (cl.Type == ClassType.Document)
            {
                fieldsRequest.Add($"[OBJECTS_MAP].[{Helpers.DateCreatedField}]");
            }
            fieldsRequest.Add($"[OBJECTS_MAP].[{Helpers.NameField}]");
            foreach (Models.Field f in fields)
            {
                fieldsRequest.Add($"[{f.Id}] AS [{f.VisibleName}]");
            }
            q.AddCustomRequest("SELECT " + string.Join(", ", fieldsRequest.ToArray()));
            q.AddCustomRequest($"FROM [{Helpers.MainTable}]");
            if (WhereCondition != null)
            {
                q.AddCustomRequest(WhereCondition);
            }
            return q.Execute();
        }
        #region Update For Correction
        public static DataTable GetSimpleObjectsWhereIdForCorrection(IClass cl, List<Guid> list, Models.Field f)
        {
            Query q = new(Helpers.MainTable, GetPathToObjectsMap(cl));
            q.OnSQLErrorDetected += OnSQLErrorDetected;
            List<string> ids = new();
            foreach (Guid id in list)
            {
                ids.Add(id.ToString());
            }
            return q.Select($"[{Helpers.IdField}], [{Helpers.NameField}] AS [Наименование объекта], [{f.Id}] AS [Значение]").WhereIn(Helpers.IdField, ids).Execute();
        }
        public async static void UpdateFieldsByIdForCorrection(IClass cl, Dictionary<string, string> fields, Models.Field f)
        {
            await Task.Run(() =>
            {
                Query q = new(Helpers.MainTable, GetPathToObjectsMap(cl));
                q.OnSQLErrorDetected += OnSQLErrorDetected;
                q.BeginTransaction();
                foreach (KeyValuePair<string, string> kvp in fields)
                {
                    Dictionary<string, string> dict = new();
                    dict.Add(f.Id.ToString(), kvp.Value);
                    q.Update(dict).WhereEqual(Helpers.IdField, kvp.Key.ToString());
                    q.SeparateCommand();
                }
                q.EndTransaction();
                q.ExecuteVoid();
            });
        }
        #endregion
        private static DataTable GetObjectsListBasic(IClass cl, string WhereCondition = null)
        {
            Query q = new("", GetPathToObjectsMap(cl));
            q.OnSQLErrorDetected += OnSQLErrorDetected;
            ClassData data = cl.GetClassData();
            List<Models.Field> fields = data.GetFieldsForMap();
            List<string> fieldsRequest = [$"[OBJECTS_MAP].[{Helpers.IdField}]"];

            if (cl.Type == ClassType.Document)
            {
                fieldsRequest.Add($"[OBJECTS_MAP].[{Helpers.DateCreatedField}]");
            }
            fieldsRequest.Add($"[OBJECTS_MAP].[{Helpers.NameField}]");
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
                    innerJoins.Add($"LEFT JOIN \"{dbName}\".[{Helpers.MainTable}] AS [{dbNamePseudoname}] ON [{dbNamePseudoname}].[{Helpers.IdField}] = [{Helpers.MainTable}].\"{f.Id}\"");
                }
                else
                {
                    fieldsRequest.Add($"[{f.Id}] AS [{f.VisibleName}]");
                }
            }

            q.AddCustomRequest("SELECT " + string.Join(", ", fieldsRequest.ToArray()));
            q.AddCustomRequest($"FROM [{Helpers.MainTable}]");
            q.AddCustomRequest(string.Join("\n", innerJoins));
            if (WhereCondition != null)
            {
                q.AddCustomRequest(WhereCondition);
            }
            return q.Execute();
        }
        #region Objects List Get
        public static DataTable GetObjectsList(IClass cl, Preset preset)
        {
            string request;
            if (preset == null)
            {
                request = "";
            }
            else
            {
                request = $"WHERE [{Helpers.MainTable}].[{Helpers.PresetField}] = '{preset.Id}'";
            }
            return GetObjectsListBasic(cl, request);
        }
        public static DataTable GetObjectsListWhereLike(IClass cl, Preset preset, string field, string value)
        {
            string request;
            if (preset == null)
            {
                request = $"WHERE [{field}] LIKE '%{value}%'";
            }
            else
            {
                request = $"WHERE [{Helpers.MainTable}].[{Helpers.PresetField}] = '{preset.Id}' AND [{field}] LIKE '%{value}%'";
            }
            return GetObjectsListBasic(cl, request);
        }
        public static DataTable GetSimpleObjectsListWhereLike(IClass cl, Preset preset, string field, string value)
        {
            string request;
            if (preset == null)
            {
                request = $"WHERE [{field}] LIKE '%{value}%'";
            }
            else
            {
                request = $"WHERE [{Helpers.MainTable}].[{Helpers.PresetField}] = '{preset.Id}' AND [{field}] LIKE '%{value}%'";
            }
            return GetObjectsListBasic(cl, request);
        }
        public static DataTable GetObjectsListWhereEqual(IClass cl, Preset preset, string field, string value)
        {
            string request;
            if (preset == null)
            {
                request = $"WHERE [{field}] = '{value}'";
            }
            else
            {
                request = $"WHERE [{Helpers.MainTable}].[{Helpers.PresetField}] = '{preset.Id}' AND [{field}] = '{value}'";
            }
            return GetObjectsListBasic(cl, request);
        }
        public static DataTable GetSimpleObjectsListWhereEqual(IClass cl, Preset preset, string field, string value)
        {
            string request;
            if (preset == null)
            {
                request = $"WHERE [{field}] = '{value}'";
            }
            else
            {
                request = $"WHERE [{Helpers.MainTable}].[{Helpers.PresetField}] = '{preset.Id}' AND [{field}] = '{value}'";
            }
            return GetObjectsListBasic(cl, request);
        }
        #endregion
        public static IObject GetObject(IClass cl, Guid id)
        {
            IObject obj;
            Query q = new(Helpers.MainTable, GetPathToObjectsMap(cl));
            q.OnSQLErrorDetected += OnSQLErrorDetected;
            ClassData classData = cl.GetClassData();
            obj = Helpers.CreateObjectByType(cl);
            DataRow dr = q.Select().WhereEqual(Helpers.IdField, id.ToString()).ExecuteOne();
            if (dr != null)
            {
                obj.Id = id;
                obj.Name = dr[Helpers.NameField].ToString();
                obj.Fields = new();
                obj.ParseServiceFields(dr);
                foreach (Models.Field f in classData.GetSavebleFields())
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
        public async static Task<List<IObject>> GetObjects(IClass cl, List<Guid> ids)
        {
            List<IObject> obj = new();
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
        //public static List<IObject> GetRelatedObjects(Class source, Class parentClass, Guid parentObject)
        //{
        //    List<IObject> objs = new();
        //    List<Models.Field> Fields = source.GetClassData().GetSavebleFields();

        //    Query q = new(Helpers.MainTable, GetPathToObjectsMap(source));
        //    q.OnSQLErrorDetected += OnSQLErrorDetected;
        //    DataTable dt = q.Select()
        //        .WhereEqual(Helpers.TargetClassField, parentClass.identifier.ToString())
        //        .WhereEqual(Helpers.TargetObjectField, parentObject.ToString()).Execute();
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        IObject obj = new();
        //        obj.Fields = new();
        //        if (dr != null)
        //        {
        //            obj.Id = Guid.Parse(dr[Helpers.IdField].ToString());
        //            obj.Name = dr[Helpers.NameField].ToString();
        //            foreach (Models.Field f in Fields)
        //            {
        //                FieldData fd = new()
        //                {
        //                    ClassField = f,
        //                    Value = dr[f.Id.ToString()].ToString()
        //                };
        //                obj.Fields.Add(fd);
        //            }
        //            objs.Add(obj);
        //        }
        //    }
        //    return objs;
        //}
        public static void RemoveObject(IClass cl, Guid id)
        {
            Query q = new(Helpers.MainTable, GetPathToObjectsMap(cl));
            q.OnSQLErrorDetected += OnSQLErrorDetected;
            q.Delete();
            q.WhereEqual(Helpers.IdField, id.ToString()).Execute();
        }
        #region Attachments & Comments
        public static void WriteComment(IClass cl, IObject target, ObjectComment comment)
        {
            comment.CreationDate = DateTime.Now;
            comment.Type = CommentType.File;
            comment.AuthorId = ProgramState.CurrentWorkspace.CurrentUser.Id;
            comment.TargetObject = target.Id;
            Dictionary<string, string> values = new()
            {
                { Helpers.IdField, comment.Id.ToString() },
                { Helpers.DateCreatedField, comment.CreationDate.ToString() },
                { Helpers.AuthorField, comment.AuthorId.ToString() },
                { Helpers.TypeField, comment.Type.ToString() },
                { Helpers.TargetObjectField, comment.TargetObject.ToString() },
                { Helpers.DataField, comment.Data }
            };

            Query q = new(Helpers.CommentsTable, GetPathToObjectsMap(cl));
            q.OnSQLErrorDetected += OnSQLErrorDetected;
            if (comment.Id == Guid.Empty) // if NEW
            {
                comment.Id = Guid.NewGuid();
                values[Helpers.IdField] = comment.Id.ToString();
                q.Insert(values);
                q.ExecuteVoid();
            }
            else // if EDIT
            {
                values.Remove(Helpers.IdField);
                q.Update(values)
                    .WhereEqual(Helpers.IdField, comment.Id.ToString())
                    .ExecuteVoid();
            }
        }
        public static async Task<List<ObjectComment>> GetObjectComments(IClass cl, IObject target)
        {
            List<ObjectComment> result = new();
            await Task.Run(() =>
            {
                Query q = new(Helpers.CommentsTable, GetPathToObjectsMap(cl));
                q.OnSQLErrorDetected += OnSQLErrorDetected;
                DataTable dt = q.Select()
                    .WhereEqual(Helpers.TargetObjectField, target.Id.ToString())
                    .Execute();
                foreach (DataRow row in dt.Rows)
                {
                    ObjectComment oc = new()
                    {
                        Id = Guid.Parse(row[Helpers.IdField].ToString()),
                        Class = cl.Id,
                        CreationDate = DateTime.Parse(row[Helpers.DateCreatedField].ToString()),
                        TargetObject = target.Id,
                        AuthorId = Guid.Parse(row[Helpers.AuthorField].ToString()),
                        Data = row[Helpers.DataField].ToString()
                    };
                    result.Add(oc);
                }
            });
            return result;
        }
        public static void RemoveObjectComment(IClass cl, ObjectComment comment)
        {
            Query q = new(Helpers.CommentsTable, GetPathToObjectsMap(cl));
            q.OnSQLErrorDetected += OnSQLErrorDetected;
            q.Delete().WhereEqual(Helpers.IdField, comment.Id.ToString()).ExecuteVoid();
        }

        #endregion
        #region Fixing
        private static void OnSQLErrorDetected(string table, ExecuteType type, int errorCode, string description)
        {

        }
        #endregion
    }
}
