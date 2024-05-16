using IncasEngine.TemplateManager;
using Incubator_2.Forms;
using Incubator_2.Models;
using Models;
using Newtonsoft.Json;
using Spire.Doc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Xceed.Document.NET;
using Xceed.Words.NET;
using Font = Xceed.Document.NET.Font;
using Formatting = Xceed.Document.NET.Formatting;
using Table = Xceed.Document.NET.Table;

namespace Common
{
    public interface ITemplator
    {
        public void Replace(List<string> tags, List<string> values, bool async = true);
        public List<SGeneratedTag> GenerateDocument(List<UC_TagFiller> tagFillers, List<TableFiller> tableFillers, string number, bool isAsync = true);
        public void CreateTable(string tag, DataTable dt);
    }

    public class WordTemplator : ITemplator
    {
        public readonly string Path;
        public WordTemplator(string path)
        {
            this.Path = path;
        }
        public WordTemplator()
        {

        }

        private string ConvertTag(string tag)
        {
            return $"[{tag}]";
        }
        private DocX LoadFile()
        {
            return DocX.Load(this.Path);
        }
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this.LoadFile());
        }
        public void DeserializeAndExtract(string input, string newPath)
        {
            DocX doc = JsonConvert.DeserializeObject<DocX>(input);
            doc.SaveAs(ProgramState.GetFullnameOfWordFile(newPath) + ".docx");
        }

        public async void Replace(List<string> tags, List<string> values, bool async = true) // не Dictionary потому что важен порядок замены
        {
            DocX doc = this.LoadFile();
            void MakeReplace()
            {
                StringReplaceTextOptions options = new();
                for (int i = 0; i < tags.Count; i++)
                {
                    options.SearchValue = this.ConvertTag(tags[i]);
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
        public List<SGeneratedTag> GenerateDocument(List<UC_TagFiller> tagFillers, List<TableFiller> tableFillers, string number, bool isAsync = true)
        {
            List<SGeneratedTag> filledTags = new();
            List<string> tagsToReplace = new() { "N" };
            List<string> values = new() { number };
            foreach (TableFiller tab in tableFillers)
            {
                this.CreateTable(tab.tag.name, tab.DataTable);
                filledTags.Add(tab.GetAsGeneratedTag());
            }
            foreach (UC_TagFiller tf in tagFillers)
            {
                int id = tf.GetId();
                string name = tf.GetTagName();
                string value = tf.GetValue();
                if (tf.tag.type != TagType.LocalConstant)
                {
                    if (tf.tag.type is TagType.Generator or TagType.Date)
                    {
                        SGeneratedTag gtg = new()
                        {
                            tag = id,
                            value = tf.GetData()
                        };
                        filledTags.Add(gtg);
                    }
                    else
                    {
                        SGeneratedTag gt = new()
                        {
                            tag = id,
                            value = value
                        };
                        filledTags.Add(gt);
                    }
                }
                tagsToReplace.Add(name);
                values.Add(value);
            }
            this.Replace(tagsToReplace, values, isAsync);
            return filledTags;
        }

        public void CreateTable(string tag, DataTable dt)
        {
            DocX doc = this.LoadFile();
            ObjectReplaceTextOptions options = new()
            {
                SearchValue = this.ConvertTag(tag)
            };
            Table tab = doc.AddTable(dt.Rows.Count + 1, dt.Columns.Count);
            tab.Design = TableDesign.TableGrid;

            Formatting head = new()
            {
                Bold = true,
                FontFamily = new Font("Times New Roman")
            };

            Formatting rowStyle = new();
            head.FontFamily = new Font("Times New Roman");
            for (int i = 0; i < dt.Columns.Count; i++) // cols
            {
                tab.Rows[0].Cells[i].Paragraphs[0].Append(dt.Columns[i].ColumnName, head);
                for (int row = 0; row < dt.Rows.Count; row++) // rows
                {
                    tab.Rows[row + 1].Cells[i].Paragraphs[0].Append(dt.Rows[row][i].ToString(), rowStyle);
                }
            }
            options.NewObject = tab;

            doc.ReplaceTextWithObject(options);
            doc.Save();
        }

        public List<string> FindAllTags()
        {
            List<string> result = new();
            DocX doc = DocX.Load(this.Path);
            Regex regex = new(@"\[[A-Za-zА-Яа-я ]*\]"); // @"\[(\w*)\]"   @"\[(\.*)\]"
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
            Spire.Doc.Document doc = new(this.Path);
            //Save the Word file as XPS and run the generated document
            string outputName = $"{ProgramState.TemplatesRuntime}\\{DateTime.Now.ToString("yyMMddHHmmssff")}.xps";
            doc.SaveToFile(outputName, FileFormat.XPS);
            File.Delete(this.Path);
            return outputName;
        }
        public void GetTextOfFile()
        {
            DocX doc = this.LoadFile();
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
