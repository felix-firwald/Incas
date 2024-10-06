using Incas.Core.Classes;
using Incas.Objects.Views.Controls;
using Spire.Doc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using WebSupergoo.WordGlue3;
using Xceed.Document.NET;
using Xceed.Words.NET;
using Font = Xceed.Document.NET.Font;
using Formatting = Xceed.Document.NET.Formatting;
using Table = Xceed.Document.NET.Table;

namespace Incas.Templates.Components
{
    public interface ITemplator
    {
        public void Replace(List<string> tags, List<string> values, bool async = true);

        public void GenerateDocument(List<FieldFiller> tagFillers, List<FieldTableFiller> tableFillers, bool async = true);

        public void CreateTable(string tag, DataTable dt);
        public List<string> FindAllTags();
        //public void CreateByObject(Incas.Objects.Components.Object obj);
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

        public async void Replace(List<string> tags, List<string> values, bool async = true) // не Dictionary потому что важен порядок замены
        {
            DocX doc = this.LoadFile();
            void MakeReplace()
            {
                StringReplaceTextOptions options = new();
                for (int i = 0; i < tags.Count; i++)
                {
                    options.SearchValue = this.ConvertTag(tags[i]);
                    options.NewValue = string.IsNullOrEmpty(values[i]) ? "" : values[i].Trim(); // а нахуя Trim?
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

        public void GenerateDocument(List<FieldFiller> tagFillers, List<FieldTableFiller> tableFillers, bool async = true)
        {
            List<string> tagsToReplace = [];
            List<string> values = [];
            foreach (FieldTableFiller tab in tableFillers)
            {
                this.CreateTable(tab.field.Name, tab.DataTable);
            }
            foreach (FieldFiller tf in tagFillers)
            {
                Guid id = tf.GetId();
                string name = tf.GetTagName();
                string value = tf.GetValue();
                tagsToReplace.Add(name);
                values.Add(value);
                if (tf.field.Type == Objects.Components.FieldType.Relation)
                {
                    foreach (Objects.Components.FieldData fd in tf.GetDataFromObjectRelation())
                    {
                        tagsToReplace.Add($"{name}.{fd.ClassField.Name}");
                        values.Add(fd.Value);
                    }
                }
            }
            this.Replace(tagsToReplace, values, async);
        }

        public void CreateByObject(Objects.Components.Object obj)
        {
            //List<string> tags = new();
            //List<string> values = new();
            //foreach (Objects.Components.FieldData item in obj.Fields)
            //{
            //    tags.Add(item.ClassField.Name);
            //    values.Add(item.Value);
            //}
        }

        public void CreateTable(string tag, DataTable dt)
        {
            DocX doc = this.LoadFile();
            ObjectReplaceTextOptions options = new()
            {
                SearchValue = this.ConvertTag(tag)
            };
            Table tab = doc.AddTable(dt.Rows.Count + 1, dt.Columns.Count);
            tab.Design = TableDesign.TableNormal;
            tab.SetBorder(TableBorderType.InsideH, new Border());
            tab.SetBorder(TableBorderType.InsideV, new Border());
            tab.SetBorder(TableBorderType.Left, new Border());
            tab.SetBorder(TableBorderType.Right, new Border());
            tab.SetBorder(TableBorderType.Top, new Border());
            tab.SetBorder(TableBorderType.Bottom, new Border());
            Formatting head = new()
            {
                Bold = true,
                FontFamily = new Font("Times New Roman"),
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
        public static void ConvertToPdf(string path)
        {

            Spire.Doc.Document doc = new(path);
            doc.SaveToFile(path, FileFormat.PDF);
        }
        public static void ConvertToPdf(List<string> paths)
        {
            foreach (string path in paths)
            {
                ConvertToPdf(path);
            }
        }

        public List<string> FindAllTags()
        {
            List<string> result = [];
            DocX doc = DocX.Load(this.Path);
            Regex regex = new(@"\[[A-Za-zА-Яа-я0-9_]*\]"); // @"\[(\w*)\]"   @"\[(\.*)\]"
            MatchCollection matches = regex.Matches(doc.Text);

            foreach (Match match in matches)
            {
                result.Add(match.Value.TrimStart('[').TrimEnd(']'));
            }
            return result;
        }

        public string TurnToXPS()
        {
            //Spire.Doc.Document doc = new(this.Path);
            Doc doc = new(this.Path);
            string outputName = $"{ProgramState.TemplatesRuntime}\\{DateTime.Now.ToString("yyMMddHHmmssff")}.xps";
            doc.SaveAs(outputName);
            //doc.SaveToFile(outputName, FileFormat.XPS);
            //File.Delete(this.Path);
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
            DialogsManager.ShowErrorDialog(doc.Text);
        }
    }
}