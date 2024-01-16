using Common;
using Incubator_2.Models;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.Common
{
    public struct SGeneratedDocument
    {
        public int id;
        public int template;
        public string templateName;
        public DateTime generatedTime;
        public string fileName;
        public string reference;
        public string destination;

        public GeneratedDocument AsModel()
        {
            GeneratedDocument d = new();
            d.id = id;
            d.template = template;
            d.templateName = templateName;
            d.generatedTime = generatedTime;
            d.fileName = fileName;
            d.reference = reference;
            d.destination = destination;
            return d;
        }
    }
    public class TemplateJSON
    {
        public int template_id;
        public string template_name;
        public string file_name;

        public Dictionary<int, string> filled_tags;
        public TemplateJSON(int templ, string templname, string file, Dictionary<int, string> tags)
        {
            template_id = templ;
            file_name = file;
            filled_tags = tags;
            template_name = templname;
        }

        public SGeneratedDocument Save()
        {
            string result = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            DateTime timestamp = DateTime.Now;
            string shortname = $"{timestamp.ToString("yyyyMMddHHmmss")}{ProgramState.GenerateSlug(4)}.jinc";
            string filename = ProgramState.TemplatesGenerated + "\\" + shortname;
            File.WriteAllText(filename, result, Encoding.UTF8);
            SGeneratedDocument st = new SGeneratedDocument();
            st.fileName = this.file_name;
            st.template = this.template_id;
            st.templateName = this.template_name;
            st.reference = shortname;
            st.generatedTime = timestamp;
            return st;
        }


    }
}
