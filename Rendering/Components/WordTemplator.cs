using DocumentFormat.OpenXml.Packaging;
using Incas.Core.Classes;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using Microsoft.Scripting.Utils;
using Spire.Doc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebSupergoo.WordGlue3;
using Xceed.Document.NET;
using Xceed.Words.NET;
using Font = Xceed.Document.NET.Font;
using Formatting = Xceed.Document.NET.Formatting;
using Table = Xceed.Document.NET.Table;
using Template = IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents.Template;

namespace Incas.Rendering.Components
{
    public class WordTemplator : ITemplator
    {
        public readonly string Path;
        private string Log = "";
        private List<string> tagsToReplace = [];
        private List<string> values = [];
        private Dictionary<string, DataTable> tables = new();
        private readonly Template template;
        public WordTemplator(Template template, string newPath)
        {
            this.template = template;
            string oldpath = ProgramState.CurrentWorkspace.GetFullnameOfDocumentFile(template.File);
            if (File.Exists(newPath))
            {
                File.Delete(newPath);
            }
            System.IO.File.Copy(oldpath, newPath, true);
            this.Path = newPath;
        }
        public WordTemplator(string newPath)
        {
            this.Path = newPath;
        }

        public WordTemplator()
        {
        }
        private void WriteLog(string log)
        {
            this.Log = this.Log + "\n" + log;
        }
        public string GetLogData()
        {
            return this.Log; 
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
            DocX doc = this.LoadFile();
            void MakeReplace()
            {
                StringReplaceTextOptions options = new();
                for (int i = 0; i < tags.Count; i++)
                {
                    options.SearchValue = this.ConvertTag(tags[i]);
                    options.NewValue = string.IsNullOrEmpty(values[i]) ? "" : values[i].Trim();
                    doc.ReplaceText(options);
                }
            }
            MakeReplace();
            doc.Save();
        }
        private void GetDataFromDocument(IncasEngine.ObjectiveEngine.Types.Documents.Document doc)
        {
            RenderData data = doc.GetDataForRender(this.template);
            this.tagsToReplace = data.TagsToReplace;
            this.values = data.Values;
            this.tables = data.Tables;
            this.SetMetaData(data);
        }
        private void ClearData()
        {
            this.tagsToReplace.Clear();
            this.values.Clear();
            this.tables.Clear();
        }
        public void GenerateDocument(IncasEngine.ObjectiveEngine.Types.Documents.Document doc)
        {
            this.GetDataFromDocument(doc);
            foreach (KeyValuePair<string, DataTable> pair in this.tables)
            {
                this.CreateTable(pair.Key, pair.Value);
            }
            this.Replace(this.tagsToReplace, this.values);
        }
        public async Task<bool> GenerateDocumentAsync(IncasEngine.ObjectiveEngine.Types.Documents.Document doc)
        {
            this.GetDataFromDocument(doc);
            await System.Threading.Tasks.Task.Run(() =>
            {
                foreach (KeyValuePair<string, DataTable> pair in this.tables)
                {
                    this.CreateTable(pair.Key, pair.Value);
                }
                this.Replace(this.tagsToReplace, this.values);
            });
            return true;
        }

        public void CreateTable(string tag, DataTable dt)
        {
            DocX doc = this.LoadFile();
            if (dt is null)
            {
                DialogsManager.ShowErrorDialog($"Таблица '{tag}' не содержит заполненной информации. Она будет пропущена при рендеринге.");
                return;
            }
            
            int table = -1;
            int row = -1;
            Dictionary<string, int> columns = new();
            Dictionary<string, Formatting> columnsFormatting = new();
            Dictionary<string, Alignment> columnsAlignments = new();
            // проходит по всем таблицам в документе
            int indextable = 0;
            foreach (Table tab in doc.Tables)
            {
                // если в таблице есть параграф с хотя бы первым тегом поиск надо прекратить
                int tableFindIndex = tab.Paragraphs.FindIndex(p => p.Text.Contains($"[{tag}."));
                if (tableFindIndex != -1)
                {
                    table = indextable;
                    int indexrow = 0;
                    // выясняем ряд
                    foreach (Row r in tab.Rows)
                    {                       
                        int rowFindIndex = r.Paragraphs.FindIndex(p => p.Text.Contains($"[{tag}."));
                        if (rowFindIndex != -1) // если найден ряд
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
                                        Formatting format = cell.Paragraphs[0].MagicText[0].formatting;
                                        Alignment align = cell.Paragraphs[0].Alignment;
                                        cell.Paragraphs[cellFindIndex].Remove(false);
                                        columns.Add(dc.ColumnName, indexcell);
                                        columnsFormatting.Add(dc.ColumnName, format);
                                        columnsAlignments.Add(dc.ColumnName, align);
                                        break;
                                    }
                                    indexcell += 1;
                                }
                            }
                            break; // прервать поиск рядов
                        }
                        indexrow += 1;
                    }
                    break;
                }
                indextable += 1;
            }
            // все что выше - ок
            if (table == -1 || row == -1)
            {
                return;
            }
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    doc.Tables[table].InsertRow(doc.Tables[table].Rows[row], row, true);
                    foreach (DataColumn dc in dt.Columns)
                    {
                        string value = dr[dc.ColumnName].ToString();
                        doc.Tables[table].Rows[row].Cells[columns[dc.ColumnName]].Paragraphs[0].Append(value, columnsFormatting[dc.ColumnName]);
                        doc.Tables[table].Rows[row].Cells[columns[dc.ColumnName]].Paragraphs[0].Alignment = columnsAlignments[dc.ColumnName];
                    }
                    row += 1;
                }
                doc.Tables[table].RemoveRow(row);
                doc.Save();
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog($"При рендеринге таблицы [{tag}] возникла ошибка:\n" + ex);
            }          
        }
        private Formatting GetFormatting(XElement rPr)
        {
            Formatting formatting = Formatting.Parse(rPr);
            if (formatting.FontFamily == null)
            {
                formatting.FontFamily = new Font("Times New Roman");
            }
            if (formatting.Size == null)
            {
                formatting.Size = 9d;
            }
            return formatting;
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
            Regex regex = new(@"\[[A-Za-zА-Яа-я0-9_]*\]");
            MatchCollection matches = regex.Matches(doc.Text);

            foreach (Match match in matches)
            {
                result.Add(match.Value.TrimStart('[').TrimEnd(']'));
            }
            return result;
        }
        public List<string> FindTableTags(string tableName) 
        {
            List<string> result = [];
            DocX doc = DocX.Load(this.Path);
            Regex regex = new(@"\[[A-Za-zА-Яа-я0-9_]*\.+[A-Za-zА-Яа-я0-9_]*\]");
            MatchCollection matches = regex.Matches(doc.Text);
            string patternTable = $"[{tableName}.";
            foreach (Match match in matches)
            {
                if (match.Value.Contains(patternTable))
                {
                    result.Add(match.Value.Replace(patternTable, "").TrimEnd(']'));
                }
            }
            return result;
        }

        private void SetMetaData(RenderData doc)
        {
            string path = this.Path;
            using (WordprocessingDocument d = WordprocessingDocument.Open(path, true))
            {
                d.PackageProperties.Creator = $"{doc.Author.Name} (через INCAS)";
                d.PackageProperties.Category = doc.Class.Name;
                d.PackageProperties.Subject = doc.Class.Component.Name;
                d.PackageProperties.Title = doc.TargetDocument.Name;
                d.PackageProperties.Revision = "1";
                d.PackageProperties.Version = DateTime.Now.ToString("yyMMddHHmm");
                d.PackageProperties.Created = doc.TargetDocument.CreationDate;
                d.PackageProperties.Modified = doc.TargetDocument.CreationDate;
                ObjectReference reference = new(doc.Class.Id, doc.TargetDocument.Id);
                d.PackageProperties.Identifier = reference.ToString();
                d.PackageProperties.LastModifiedBy = $"{ProgramState.CurrentWorkspace.CurrentUser.Name} (через INCAS)";
                d.PackageProperties.Description = "Этот документ создан в программе INCAS";
            }
        }
        public async static Task<string> GetMetaDataIdentifier(string path)
        {
            string result = "";
            await System.Threading.Tasks.Task.Run(() =>
            {
                using (WordprocessingDocument d = WordprocessingDocument.Open(path, true))
                {
                    result = d.PackageProperties.Identifier;
                }
            });
            return result;
        }
        public static string TurnToPDF(string file)
        {
            Doc doc = new(file);
            string outputName = file.Replace(".docx", ".pdf");
            doc.SaveAs(outputName);
            return outputName;
        }
        public static string ReplaceToPDF(string file)
        {
            Doc doc = new(file);
            string outputName = file.Replace(".docx", ".pdf");
            doc.SaveAs(outputName);
            if (File.Exists(file))
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {

                }
            }
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