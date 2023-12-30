using System.Collections.Generic;
using Xceed.Words.NET;
using Xceed.Document.NET;
using System.Text.RegularExpressions;
using Spire.Doc;
using System;
using System.IO;

namespace Common
{
    public class WordTemplator
    {
        public readonly string Path;
        public WordTemplator(string path) 
        {
            this.Path = path;
        }

        private string ConvertTag(string tag)
        {
            return $"[{tag}]";
        }
        private DocX LoadFile()
        {
            return DocX.Load(this.Path);
        }

        public void Replace(List<string> tags, List<string> values) // не Dictionary потому что важен порядок замены
        {
            DocX doc = LoadFile();
            StringReplaceTextOptions options = new StringReplaceTextOptions();
            for (int i = 0; i < tags.Count; i++)
            {
                options.SearchValue = ConvertTag(tags[i]);
                options.NewValue = values[i].Trim(); // а нахуя Trim?
                doc.ReplaceText(options);
            }
            doc.Save();
            
        }

        public List<string> FindAllTags()
        {
            List<string> result = new List<string>();
            DocX doc = DocX.Load(this.Path);
            Regex regex = new Regex(@"\[[A-Za-zА-Яа-я ]*\]"); // @"\[(\w*)\]"   @"\[(\.*)\]"
            MatchCollection matches = regex.Matches(doc.Text);
            foreach (Match match in matches)
            {
                result.Add(match.Value.TrimStart('[').TrimEnd(']'));
            }
            return result;
        }
        public string TurnToXPS()
        {
            //Create a Document class object and load the sample file
            Spire.Doc.Document doc = new Spire.Doc.Document(this.Path);
            //Save the Word file as XPS and run the generated document
            string outputName = $"{ProgramState.TemplatesRuntime}\\{DateTime.Now.ToString("yyMMddHHmmssff")}.xps";
            doc.SaveToFile(outputName, FileFormat.XPS);
            File.Delete(this.Path);
            return outputName;
        }
        public void GetTextOfFile()
        {
            DocX doc = LoadFile();
            IList<Xceed.Document.NET.Section> s = doc.GetSections();
            string result = "";
            foreach (Xceed.Document.NET.Section section in s)
            {
                result = result + "\n" + section.ToString();
            }
            ProgramState.ShowErrorDialog(doc.Text);
        }
    }
}
