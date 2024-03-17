using System.Collections.Generic;
using Xceed.Words.NET;
using Xceed.Document.NET;
using System.Text.RegularExpressions;
using Spire.Doc;
using System;
using System.IO;
using System.Data;
using System.Windows.Automation.Peers;
using Table = Xceed.Document.NET.Table;

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

        public async void Replace(List<string> tags, List<string> values, bool async = true) // не Dictionary потому что важен порядок замены
        {
            DocX doc = LoadFile();
            void MakeReplace()
            {
                StringReplaceTextOptions options = new StringReplaceTextOptions();
                for (int i = 0; i < tags.Count; i++)
                {
                    options.SearchValue = ConvertTag(tags[i]);
                    options.NewValue = values[i].Trim(); // а нахуя Trim?
                    doc.ReplaceText(options);
                }
                // MakeFormatting(doc);
            }
            if (async)
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    MakeReplace();
                });
            }
            else
            {
                MakeReplace();
            }
            doc.Save();
        }
        //public void MakeFormatting(DocX doc)
        //{
        //    StringReplaceTextOptions options = new StringReplaceTextOptions();
        //    options.NewFormatting = new Formatting();
        //    options.NewFormatting.FontFamily = new Font("Times New Roman");
        //    options.NewFormatting.Bold = true;
        //    GetMatchesFormat(doc, @"<b>.*</b>", new List<string> { "<b>", "</b>" }, options);
        //    options.NewFormatting.Bold = false;
        //    options.NewFormatting.Italic = true;
        //    GetMatchesFormat(doc, @"<i>.*</i>", new List<string> { "<i>", "</i>" }, options);
        //    doc.ReplaceText(options);
        //}
        //private List<string> GetMatchesFormat(DocX doc, string pattern, List<string> removableParts, StringReplaceTextOptions options)
        //{
        //    Regex reg = new Regex(pattern);
        //    List<string> result = new List<string>();
        //    MatchCollection matches = reg.Matches(doc.Text);
        //    foreach (Match match in matches)
        //    {
        //        result.Add(match.Value);
        //        options.SearchValue = match.Value;
        //        string newValue = match.Value;
        //        foreach (string part in removableParts)
        //        {
        //            newValue = newValue.Replace(part, "");
        //        }
        //        options.NewValue = newValue;
        //        doc.ReplaceText(options);
        //    }

        //    return result;
        //}
        public void CreateTable(string tag, DataTable dt)
        {
            DocX doc = LoadFile();
            ObjectReplaceTextOptions options = new ObjectReplaceTextOptions();
            options.SearchValue = ConvertTag(tag);
            Table tab = doc.AddTable(dt.Rows.Count + 1, dt.Columns.Count);
            tab.Design = TableDesign.TableGrid;

            Formatting head = new Formatting();
            head.Bold = true;
            head.FontFamily = new Font("Times New Roman");

            Formatting rowStyle = new Formatting();
            head.FontFamily = new Font("Times New Roman");
            for (int i = 0; i < dt.Columns.Count; i++) // cols
            {
                tab.Rows[0].Cells[i].Paragraphs[0].Append(dt.Columns[i].ColumnName, head);
                for (int row = 0; row < dt.Rows.Count; row++) // rows
                {
                    tab.Rows[row+1].Cells[i].Paragraphs[0].Append(dt.Rows[row][i].ToString(), rowStyle);
                }
            }
            options.NewObject = tab;

            doc.ReplaceTextWithObject(options);
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
