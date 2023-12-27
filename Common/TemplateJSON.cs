using Common;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.Common
{
    public struct STemplateJSON
    {
        public int template_id;
        public string template_name;
        public DateTime generated_time;
        public string file_name;
        public string reference;
        public string destination;
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

        public STemplateJSON Save(int generationId)
        {
            string result = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            DateTime timestamp = DateTime.Now;
            string shortname = $"{timestamp.ToString("yyyyMMddHHmmss")}_{generationId}.jinc";
            string filename = ProgramState.TemplatesGenerated + "\\" + shortname;
            File.WriteAllText(filename, result, Encoding.UTF8);
            STemplateJSON st = new STemplateJSON();
            st.file_name = this.file_name;
            st.template_id = this.template_id;
            st.template_name = this.template_name;
            st.reference = shortname;
            st.generated_time = timestamp;
            return st;
        }


    }
}
