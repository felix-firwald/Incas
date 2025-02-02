using Incas.Core.AutoUI;
using Incas.Core.Exceptions;
using Incas.Core.Models;
using Incas.Objects.ServiceClasses.Groups.Components;
using Incas.Objects.ServiceClasses.Users.Components;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Incas.Core.Classes
{
    internal class Workspace
    {
        private const string RootPostfix = @"\Root";
        private const string ServiceDatabasePostfix = @"\service.incas";
        private const string ObjectsPostfix = @"\Objects";
        private const string ServerProcessesPostfix = @"\ServerProcesses";
        private const string ExchangesPostfix = @"\Exchanges";
        private const string ScriptsPostfix = @"\Scripts";
        private const string LogDataPostfix = @"\LogData";
        private const string TemplatesSourcesPostfix = @"\Templates\Sources";
        private const string TemplatesRuntimePostfix = @"\Templates\Runtime";
        public const string WorkspaceDataName = "DEFINITION";
        /// <summary>
        /// Target disk location where the workspace is located if type of connection is disk-connection, overwise contains special cymbol for replacement on server-side
        /// </summary>
        private string CommonPath { get; set; }

        /// <summary>
        /// A main folder contains workspace-service folders. Never use it directly except to calculate other folders
        /// </summary>
        private string Root { get; set; }

        /// <summary>
        /// A path to service.incas if type of connection is disk-connection, overwise contains special cymbol for replacement on server-side
        /// </summary>
        internal string ServiceDatabasePath { get; set; }

        /// <summary>
        /// A folder contains all databases for all classes
        /// </summary>
        internal string ObjectsPath { get; set; }

        /// <summary>
        /// Outdated functionality
        /// </summary>
        private string ServerProcesses { get; set; }

        /// <summary>
        /// A folder intended for data exchange between users
        /// </summary>
        private string Exchanges { get; set; }

        /// <summary>
        /// A folder contains all scripts used in workspace (like common scripts, classes scripts, etc.)
        /// </summary>
        private string Scripts { get; set; }

        /// <summary>
        /// A folder contains logging data
        /// </summary>
        private string LogData { get; set; }

        /// <summary>
        /// A folder contains all templates with supported file-types such as *.docx, *.xlsx
        /// </summary>
        private string TemplatesSources { get; set; }

        /// <summary>
        /// A folder intended for render documents
        /// </summary>
        private string TemplatesRuntime { get; set; }

        /// <summary>
        /// A visible name of Workspace
        /// </summary>
        public string WorkspaceName { get; private set; }

        #region Service Classes
        private User currentUser { get; set; }
        internal User CurrentUser 
        { 
            get
            {
                return this.currentUser;
            }
            set
            {
                this.currentUser = value;
                this.CurrentGroup = value.GetGroup();
                this.OnUserChanged?.Invoke(value);
                this.OnGroupChanged?.Invoke(this.CurrentGroup);
            }
        }
        internal Group CurrentGroup { get; private set; }
        internal delegate void UpdatedUser(User user);
        internal delegate void UpdatedGroup(Group group);
        internal delegate void UpdatedDefinition(WorkspacePrimarySettings def);
        internal event UpdatedUser OnUserChanged;
        internal event UpdatedGroup OnGroupChanged;
        internal event UpdatedDefinition OnDefinitionChanged;
        #endregion

        private Guid definitionId { get; set; }
        private WorkspacePrimarySettings definitionCache { get; set; }

        private DateTime LastGarbageCollect {  get; set; }

        internal Workspace(string workspacePath)
        {
            this.CommonPath = workspacePath;
            this.CalculateFoldersPaths();
            this.SetDirs();
        }
        internal WorkspacePrimarySettings GetDefinition(bool refresh = false)
        {
            if (refresh)
            {
                this.definitionCache = null;
            }
            if (this.definitionCache is null)
            {                
                try
                {
                    using Parameter par = ProgramState.GetParameter(ParameterType.WORKSPACE, WorkspaceDataName, "", false);
                    this.definitionId = par.Id;
                    return JsonConvert.DeserializeObject<WorkspacePrimarySettings>(par.Value);
                }
                catch (Exception ex)
                {
                    throw new BadWorkspaceException("Определение рабочего пространства повреждено: не удалось распаковать параметр [WORKSPACE:DEFINITION] рабочего пространства.\n" + ex.Message);
                }
            }
            return this.definitionCache;
        }
        internal void UpdateDefinition(WorkspacePrimarySettings settings)
        {
            Parameter p = new(this.definitionId)
            {
                Value = JsonConvert.SerializeObject(settings)
            };
            p.UpdateValue();
        }
        internal async void InitializeDefinition(CreateWorkspace data)
        {
            await Task.Run(() =>
            {
                ProgramStatusBar.ShowLoadingWindow("Создание рабочего пространства", "Это займет немного времени");
                DatabaseManager.InitializeService();
                this.definitionCache = new()
                {
                    Id = Guid.NewGuid(),
                    Name = data.WorkspaceName
                };
                Objects.ServiceClasses.Components.InitializationManager.RunInitialization(this.definitionCache, data.Password);
                Parameter p = new()
                {
                    Type = ParameterType.WORKSPACE,
                    Name = Workspace.WorkspaceDataName,
                    Value = JsonConvert.SerializeObject(this.definitionCache)
                };
                p.CreateParameter();             
            });
            ProgramStatusBar.HideLoadingWindow();
        }
        internal bool LogIn(string password)
        {
            bool result = false;
            if (this.CurrentUser.Data.Password == password)
            {
                result = true;
            }
            return result;
        }
        private void CalculateFoldersPaths()
        {
            this.Root = this.CommonPath + RootPostfix;
            this.ServiceDatabasePath = this.CommonPath + ServiceDatabasePostfix;
            this.ObjectsPath = this.Root + ObjectsPostfix;
            this.ServerProcesses = this.Root + ServerProcessesPostfix;
            this.Exchanges = this.Root + ExchangesPostfix;
            this.Scripts = this.Root + ScriptsPostfix;
            this.LogData = this.Root + LogDataPostfix;
            this.TemplatesRuntime = this.Root + TemplatesRuntimePostfix;
            this.TemplatesSources = this.Root + TemplatesSourcesPostfix;
        }
        private void SetDirs()
        {
            Directory.CreateDirectory(this.Root);
            Directory.CreateDirectory(this.ObjectsPath);
            Directory.CreateDirectory(this.ServerProcesses);
            Directory.CreateDirectory(this.Exchanges);
            Directory.CreateDirectory(this.Scripts);
            Directory.CreateDirectory(this.LogData);
            Directory.CreateDirectory(this.TemplatesRuntime);
            Directory.CreateDirectory(this.TemplatesSources);
        }
        internal async void ClearRuntimeFiles()
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                foreach (string item in Directory.GetFiles(this.TemplatesRuntime))
                {
                    try
                    {
                        File.Delete(item);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            });
        }
        internal async void CollectGarbage()
        {
            await Task.Run(() =>
            {
                if (this.LastGarbageCollect < DateTime.Now.AddMinutes(-5))
                {
                    ProgramStatusBar.SetText("Выполняется сборка мусора...");
                    this.RemoveFilesOlderThan(this.ServerProcesses, DateTime.Now.AddHours(-8));
                    this.RemoveFilesOlderThan(this.Exchanges, DateTime.Now.AddHours(-1));
                    this.RemoveFilesOlderThan(this.TemplatesRuntime, DateTime.Now.AddHours(-1));
                    this.RemoveFilesOlderThan(this.LogData, DateTime.Now.AddHours(-8));
                    this.LastGarbageCollect = DateTime.Now;
                    ProgramStatusBar.Hide();
                }
            });

        }
        private async void RemoveFilesOlderThan(string folder, DateTime time)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                foreach (string item in Directory.GetFiles(folder))
                {
                    if (File.GetCreationTime(item) < time)
                    {
                        try
                        {
                            File.Delete(item);
                        }
                        catch { }
                    }
                }
            });
        }
        private void CopyFileToWorkspace(string source, string targetFolder, string name)
        {
            try
            {
                File.Copy(source, targetFolder + "\\" + name);
            }
            catch { }
        }
        private void CopyFileToWorkspace(string source, string targetFolder)
        {
            string result = Path.GetFileName(source);
            this.CopyFileToWorkspace(source, targetFolder, result);
        }
        #region Objects

        #endregion
        #region Scripts
        private string CalculateScriptPath(Guid id)
        {
            return this.Scripts + $"\\{id}.py";
        }
        /// <summary>
        /// Gets an existing script from <see cref="Scripts"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Content of *.py file</returns>
        public string GetScript(Guid id)
        {
            if (id == Guid.Empty)
            {
                return "";
            }
            try
            {
                return File.ReadAllText(this.CalculateScriptPath(id));
            }
            catch (FileNotFoundException)
            {
                DialogsManager.ShowErrorDialog("Не найден файл скрипта.");
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }
        /// <summary>
        /// Write script into file *.py to <see cref="Scripts"/>
        /// </summary>
        /// <param name="content"></param>
        /// <returns>Generated <see cref="Guid"/> of script</returns>
        public Guid SetScript(string content)
        {
            Guid id = Guid.NewGuid();
            File.WriteAllTextAsync(this.CalculateScriptPath(id), content);
            return id;
        }
        /// <summary>
        /// Edits an existing script
        /// </summary>
        /// <param name="id"></param>
        /// <param name="content"></param>
        /// <returns>Given as argument <see cref="Guid"/> of script</returns>
        public Guid SetScript(Guid id, string content)
        {
            File.WriteAllTextAsync(this.CalculateScriptPath(id), content);
            return id;
        }
        /// <summary>
        /// Removes script
        /// </summary>
        /// <param name="id"></param>
        public void RemoveScript(Guid id)
        {
            try
            {
                File.Delete(this.CalculateScriptPath(id));
            }
            catch { }
        }
        #endregion
        #region Templates
        public void SetTemplate(string path)
        {
            this.CopyFileToWorkspace(path, this.TemplatesSources);
        }
        /// <summary>
        /// Gets full path with workspace to template file
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A path</returns>
        public string GetFullnameOfDocumentFile(string name)
        {
            return this.TemplatesSources + "\\" + name;
        }
        /// <summary>
        /// Checks if file with such name exists in <see cref="TemplatesSources">workspace folder</see>
        /// </summary>
        /// <returns>Boolean result of operation</returns>
        public bool HasTemplateWithSuchName(string path)
        {
            return File.Exists(this.GetFullnameOfDocumentFile(path));
        }
        /// <summary>
        /// Compare paths with target file and file into the <see cref="TemplatesSources">workspace folder</see> 
        /// </summary>
        /// <returns>Boolean result of operation</returns>
        public bool IsTemplatePathContainsWorkspacePath(string path)
        {
            return path.StartsWith(this.TemplatesSources);
        }
        public string GetSourcesTemplatesFolder()
        {
            return this.TemplatesSources;
        }
        public string GetRuntimesTemplatesFolder()
        {
            return this.TemplatesRuntime;
        }
        #endregion
    }
}
