using Common;
using DocumentFormat.OpenXml.Bibliography;
using Incubator_2.Models;
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
        public static void GetRegistry()
        {
            using (GeneratedDocument d = new())
            {

            }
        }
        public static void AddRecord(TemplateJSON record)
        {
            record.Save().AsModel().AddRecord();
        }
        private static string GetReference(string filename)
        {
            return ProgramState.TemplatesGenerated + "\\" + filename;
        }
        public static TemplateJSON LoadRecord(SGeneratedDocument refer)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<TemplateJSON>(File.ReadAllText(GetReference(refer.reference)));
        }
        public async static void RemoveRecords(List<SGeneratedDocument> record)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                List<string> ids = new();
                foreach (SGeneratedDocument r in record)
                {
                    File.Delete(GetReference(r.reference));
                    ids.Add(r.id.ToString());
                }
                using (GeneratedDocument d = new())
                {
                    d.RemoveRecords(ids);
                }
            });
        }
    }
}
