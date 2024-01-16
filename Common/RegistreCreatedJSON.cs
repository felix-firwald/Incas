using Common;
using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.Common
{
    public static class RegistreCreatedJSON
    {
        static string mainFile { get { return ProgramState.TemplatesGenerated + "\\reg.jinc"; } }
        public static List<SGeneratedDocument> generatedDocuments { get; private set; }
        
        static RegistreCreatedJSON()
        {
            generatedDocuments = new List<SGeneratedDocument>();
        }
        public static void GetRegistry()
        {
            if (File.Exists(mainFile))
            {
                generatedDocuments = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SGeneratedDocument>>(File.ReadAllText(mainFile));
                if (generatedDocuments == null )
                {
                    generatedDocuments = new List<SGeneratedDocument>();
                }
            }
            else
            {
                File.Create(mainFile);
                generatedDocuments = new List<SGeneratedDocument>();
            }
        }
        public static void SaveRegistry()
        {
            if (!File.Exists(mainFile))
            {
                File.Create(mainFile);
            }
            File.WriteAllText(mainFile, Newtonsoft.Json.JsonConvert.SerializeObject(generatedDocuments));
        }
        public static void AddRecord(TemplateJSON record)
        {
            generatedDocuments.Add(record.Save(generatedDocuments.Count + 1));
        }
        private static string GetReference(string filename)
        {
            return ProgramState.TemplatesGenerated + "\\" + filename;
        }
        public static TemplateJSON LoadRecord(SGeneratedDocument refer)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<TemplateJSON>(File.ReadAllText(GetReference(refer.reference)));
        }
        public static void RemoveRecord(SGeneratedDocument record)
        {
            generatedDocuments.Remove(record);
            File.Delete(GetReference(record.reference));
        }
    }
}
