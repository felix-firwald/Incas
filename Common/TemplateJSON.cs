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
    public class TemplateJSON
    {
        public int template_id;
        public string file_name;
        public DateTime generated_time;
        public Dictionary<int, string> filled_tags;
        public int using_slug;
        public TemplateJSON(int templ, string file, Dictionary<int, string> tags)
        {
            template_id = templ;
            file_name = file;
            filled_tags = tags;
            generated_time = DateTime.Now;
            Random rnd = new Random();
            using_slug = rnd.Next(101, 999);
        }

        public void Convert()
        {
            string result = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            string filename = $"{ProgramState.TemplatesGenerated}\\{generated_time.ToString("yyyyMMddHHmmss")}_{using_slug}.jinc";
            File.WriteAllText(filename, result, Encoding.UTF8);
        }


    }
}
