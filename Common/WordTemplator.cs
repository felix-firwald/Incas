using System.Collections.Generic;
using Xceed.Words.NET;
using Xceed.Document.NET;
using System.Text.RegularExpressions;


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

        public void Replace(string tag, string value)
        {
            DocX doc = LoadFile();
            StringReplaceTextOptions options = new StringReplaceTextOptions();
            options.SearchValue = ConvertTag(tag);
            options.NewValue = value.Trim();
            doc.ReplaceText(options);
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
        public void GetTextOfFile()
        {
            DocX doc = LoadFile();
            IList<Section> s = doc.GetSections();
            string result = "";
            foreach (Section section in s)
            {
                result = result + "\n" + section.ToString();
                
            }
            ProgramState.ShowErrorDialog(doc.Text);
        }
    }
}
