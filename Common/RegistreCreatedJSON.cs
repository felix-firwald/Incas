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

        public static string GetReference(int template, string filename)
        {
            string dir = $"{ProgramState.TemplatesGenerated}\\{template}";
            Directory.CreateDirectory(dir);
            return dir + $"\\{filename}.jinc";
        }

        public async static void RemoveRecords(List<SGeneratedDocument> record)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                List<string> ids = new();
                foreach (SGeneratedDocument r in record)
                {
                    File.Delete(GetReference(r.template, r.reference));
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
