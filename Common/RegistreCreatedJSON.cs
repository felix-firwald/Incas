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
        public static List<STemplateJSON> generatedDocuments { get; private set; }
        
        static RegistreCreatedJSON()
        {
            generatedDocuments = new List<STemplateJSON>();
        }
        public static void GetRegistry()
        {
            if (File.Exists(mainFile))
            {
                generatedDocuments = Newtonsoft.Json.JsonConvert.DeserializeObject<List<STemplateJSON>>(File.ReadAllText(mainFile));
            }
            else
            {
                File.Create(mainFile);
                generatedDocuments = new List<STemplateJSON>();
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
            if (generatedDocuments is null)
            {
                generatedDocuments = new List<STemplateJSON>();
            }
            generatedDocuments.Add(record.Save(generatedDocuments.Count + 1));
        }
        public static void RemoveRecord(STemplateJSON record)
        {
            generatedDocuments.Remove(record);
            File.Delete(record.reference);
        }
    }
}
