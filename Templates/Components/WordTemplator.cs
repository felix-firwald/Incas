using DocumentFormat.OpenXml.ExtendedProperties;
using Incas.Core.Classes;
using Incas.Objects.Views.Controls;
using Microsoft.Scripting.Utils;
using Spire.Doc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using WebSupergoo.WordGlue3;
using Xceed.Document.NET;
using Xceed.Words.NET;
using Font = Xceed.Document.NET.Font;
using Formatting = Xceed.Document.NET.Formatting;
using Table = Xceed.Document.NET.Table;

namespace Incas.Templates.Components
{
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
                this.CreateTable(tab.Field.Name, tab.GetValue());
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

        public void CreateTable(string tag, DataTable dt)
        {
            DocX doc = this.LoadFile();
            int table = -1;
            int row = -1;
            Dictionary<string, int> columns = new();
            Formatting rowStyle = new();
            rowStyle.FontFamily = new Font("Times New Roman");
            // проходит по всем таблицам в документе
            foreach (Table tab in doc.Tables)
            {
                // если в таблице есть параграф с хотя бы первым тегом поиск надо прекратить
                int indextable = tab.Paragraphs.FindIndex(p => p.Text.Contains($"[{tag}."));
                
                if (indextable != -1)
                {
                    table = tab.Index;
                    int indexrow = 0;
                    // выясняем ряд
                    foreach (Row r in tab.Rows)
                    {
                        
                        int rowFindIndex = r.Paragraphs.FindIndex(p => p.Text.Contains($"[{tag}."));
                        if (rowFindIndex != -1)
                        {
                            row = indexrow;
                            foreach (DataColumn dc in dt.Columns)
                            {
                                int indexcell = 0;
                                foreach (Cell cell in r.Cells)
                                {
                                    int cellFindIndex = cell.Paragraphs.FindIndex(p => p.Text.Contains($"[{tag}.{dc.ColumnName}]"));
                                    if (cellFindIndex != -1)
                                    {
                                        cell.Paragraphs[cellFindIndex].Remove(false);
                                        columns.Add(dc.ColumnName, indexcell);
                                        break;
                                    }
                                    indexcell += 1;
                                }
                            }
                            break;
                        }
                        indexrow += 1;
                    }
                    break;
                }
            }
            if (table == -1 || row == -1)
            {
                return;
            }
          
            foreach (DataRow dr in dt.Rows)
            {
                doc.Tables[table].InsertRow(doc.Tables[table].Rows[row], row, true);
                foreach (DataColumn dc in dt.Columns)
                {
                    string value = dr[dc.ColumnName].ToString();
                    doc.Tables[table].Rows[row].Cells[columns[dc.ColumnName]].Paragraphs[0].Append(value, rowStyle);
                }
                row += 1;
            }
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