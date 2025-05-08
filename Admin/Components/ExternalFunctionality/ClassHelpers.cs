using Incas.Core.Classes;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using IncasEngine.ObjectiveEngine.Types.Events.ClassComponents;
using IncasEngine.ObjectiveEngine.Types.Processes.ClassComponents;
using IncasEngine.Workspace;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Admin.Components.ExternalFunctionality
{
    public class ClassHelpers
    {
        public struct ClassDraft
        {
            public bool IsRealRecord { get; set; }
            public DateTime DateSaved { get; set; }
            public string InternalName { get; set; }
            public string Description { get; set; }
            public string Name { get; set; }
            public DocumentClassData DocumentClassData { get; set; }
            public ClassData ClassData { get; set; }
            public ProcessClassData ProcessClassData { get; set; }
            public EventClassData EventClassData { get; set; }
        }
        public static void SaveClassDraft(IClass cl, ClassDraft draft)
        {
            draft.IsRealRecord = true;
            draft.DateSaved = DateTime.Now;
            string content = JsonConvert.SerializeObject(draft, IncasEngine.Core.EngineGlobals.JsonSerializerSettings);
            File.WriteAllText(GetFileName(cl), content);
        }
        public static ClassDraft LoadClassDraft(IClass cl)
        {
            string content = File.ReadAllText(GetFileName(cl));
            return JsonConvert.DeserializeObject<ClassDraft>(content);           
        }
        private static string GetFileName(IClass cl)
        {
            return WorkspacePaths.UserPathCache + $"{ProgramState.CurrentWorkspace.GetDefinition().Id.ToString("N")}" +
                $"{cl.Id.ToString("N")}";
        }
    }
}
