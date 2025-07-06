using ClosedXML.Excel;
using Incas.Core.Classes;
using IncasEngine.Core.ExtensionMethods;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Types.Documents;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Incas.Rendering.Components
{
    public class ExcelTemplator : ITemplator
    {
        private string Log = "";
        private List<string> tagsToReplace = [];
        private List<string> values = [];
        private Dictionary<string, DataTable> tables = new();
        private readonly Template template;
        private string path;
        public XLWorkbook workbook;
        public IXLWorksheet worksheet;
        public ExcelTemplator(Template template, string newPath)
        {
            this.template = template;
            this.path = newPath;
            string oldpath = ProgramState.CurrentWorkspace.GetFullnameOfDocumentFile(template.File);
            if (File.Exists(newPath))
            {
                File.Delete(newPath);
            }
            System.IO.File.Copy(oldpath, newPath, true);
            this.workbook = new XLWorkbook(newPath);
            this.worksheet = this.workbook.Worksheet(1);
        }
        public ExcelTemplator(string newPath)
        {
            this.path = newPath;
            this.workbook = new XLWorkbook(newPath);
            this.worksheet = this.workbook.Worksheet(1);
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
        public void Replace(List<string> tags, List<string> values)
        {
            void MakeReplace()
            {
                for (int i = 0; i < tags.Count; i++)
                {
                    IXLCells c = this.worksheet.Search(this.ConvertTag(tags[i]));
                    foreach (IXLCell item in c)
                    {
                        item.Value = item.Value.GetText().Replace(this.ConvertTag(tags[i]), values[i]);
                    }
                }
            }
            MakeReplace();
            this.workbook.Save();
        }
        public void CreateTable(string tag, DataTable dt)
        {
            int rowIndex = -1;
            int lastColumnIndex = -1;
            int firstColumnIndex = -1;
            if (this.worksheet is null)
            {
                return;
            }
            if (dt is null)
            {
                DialogsManager.ShowErrorDialog($"Таблица '{tag}' не содержит заполненной информации.");
                return;
            }
            Dictionary<string, IXLCell> columns = new();
            IXLCell cell = this.worksheet.Search($"[{tag}.", System.Globalization.CompareOptions.IgnoreCase, false).FirstOrDefault();
            if (cell is null)
            {
                return;
            }
            rowIndex = cell.WorksheetRow().RowNumber();
            foreach (DataColumn dc in dt.Columns)
            {
                try
                {
                    IXLCell columnCell = this.worksheet.Search($"[{tag}.{dc.ColumnName}]", System.Globalization.CompareOptions.IgnoreCase, false)?.FirstOrDefault();
                    if (columnCell is not null)
                    {
                        int col = columnCell.WorksheetColumn().ColumnNumber();
                        if (col > lastColumnIndex)
                        {
                            lastColumnIndex = col;
                        }
                        if (col < firstColumnIndex)
                        {
                            firstColumnIndex = col;
                        }
                        columns.Add(dc.ColumnName, columnCell);
                    }
                }
                catch { }
            }
            double rowHeight = 15;
            if (dt.Rows.Count > 1)
            {
                IXLRow firstRowInstance = this.worksheet.Row(rowIndex); // добавлена переменная
                rowHeight = firstRowInstance.Height;
                firstRowInstance.InsertRowsBelow(dt.Rows.Count - 1);
            }
            foreach (DataRow dr in dt.Rows)
            {
                this.worksheet.Row(rowIndex).Height = rowHeight;
                foreach (DataColumn dc in dt.Columns)
                {
                    try
                    {                       
                        string value = dr[dc.ColumnName].ToString();
                        int currentCol = columns[dc.ColumnName].WorksheetColumn().ColumnNumber();
                        this.worksheet.Cell(rowIndex, currentCol).SetValue(value);
                        this.worksheet.Cell(rowIndex, currentCol).Style = columns[dc.ColumnName].Style;
                        if (columns[dc.ColumnName].IsMerged())
                        {
                            IXLRangeAddress range = columns[dc.ColumnName].MergedRange().RangeAddress;
                            if (range.ColumnSpan > 1)
                            {
                                this.worksheet.Range(rowIndex, currentCol, rowIndex, currentCol + range.ColumnSpan - 1).Merge();
                            }
                        }
                    }
                    catch { }         
                }
                rowIndex += 1;
            }
            this.workbook.Save();
        }
        
        private void GetDataFromDocument(Document doc)
        {
            RenderData data = doc.GetDataForRender(this.template);
            this.tagsToReplace = data.TagsToReplace;
            this.values = data.Values;
            this.tables = data.Tables;
            this.SetMetaData(data);
        }
        private void SetMetaData(RenderData doc)
        {
            this.workbook.Properties.Manager = $"Incas";
            this.workbook.Properties.Author = doc.Author.Name;
            this.workbook.Properties.Category = doc.Class.Name;
            this.workbook.Properties.Subject = doc.Class.Component.Name;
            this.workbook.Properties.Title = doc.TargetDocument.Name;
            this.workbook.Properties.Created = doc.TargetDocument.CreationDate;
            this.workbook.Properties.Modified = doc.TargetDocument.CreationDate;            
            ObjectReference reference = new(doc.Class.Id, doc.TargetDocument.Id);
            this.workbook.Properties.LastModifiedBy = $"{ProgramState.CurrentWorkspace.CurrentUser.Name} (через Incas)";
            this.workbook.Properties.Company = ProgramState.CurrentWorkspace.GetDefinition().Name;
            this.workbook.Properties.Comments = $"Этот документ создан с использованием программы по автоматизации бизнес-процессов — Incas {ProgramState.Edition}, версия: {ProgramState.Version}.\nРазработчик программы: @felixfirwald";            
            this.workbook.CustomProperties.Add(FileTemplator.ObjectReferenceProperty, reference.ToString());
        }
        public async static Task<string> GetObjectReference(string path)
        {
            string result = "";
            await System.Threading.Tasks.Task.Run(() =>
            {
                IXLWorkbook workbook = new XLWorkbook(path);
                result = workbook.CustomProperty(FileTemplator.ObjectReferenceProperty).Value.ToString();
            });
            return result;
        }
        public void GenerateDocument(Document doc)
        {
            this.GetDataFromDocument(doc);
            foreach (KeyValuePair<string, DataTable> pair in this.tables)
            {
                this.CreateTable(pair.Key, pair.Value);
            }
            this.Replace(this.tagsToReplace, this.values);
        }
        public async Task<bool> GenerateDocumentAsync(Document doc)
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

        public List<string> FindAllTags()
        {
            string source = "";
            List<string> result = [];
            Regex regex = new(@"\[[A-Za-zА-Яа-я0-9_]*\]"); // @"\[(\w*)\]"   @"\[(\.*)\]"
            foreach (IXLCell cell in this.worksheet.CellsUsed())
            {
                string value = cell.Value.ToString();
                if (value.Contains('['))
                {
                    source = source + "\n" + value;
                }
            }
            MatchCollection matches = regex.Matches(source);
            foreach (Match match in matches)
            {
                result.Add(match.Value.TrimStart('[').TrimEnd(']'));
            }
            return result.ReturnUnique();
        }
        public List<string> FindTableTags(string tableName)
        {
            string source = "";
            List<string> result = [];
            Regex regex = new(@"\[[A-Za-zА-Яа-я0-9_]*\.+[A-Za-zА-Яа-я0-9_]*\]");
            foreach (IXLCell cell in this.worksheet.CellsUsed())
            {
                string value = cell.Value.ToString();
                if (value.Contains('['))
                {
                    source = source + "\n" + value;
                }
            }
            MatchCollection matches = regex.Matches(source);
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
        /// <summary>
        /// Input dictionary description: key is real name of <see cref="Objects.Models.Field"/>,
        /// value is searching name for <see cref="Objects.Models.Field"/>.
        /// Output dictionary description: key is real name of field, 
        /// value is value.
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public async static Task<List<Dictionary<string,string>>> GetFromFile(string file, Dictionary<string,string> map)
        {
            List<Dictionary<string, string>> result = new();
            await Task.Run(() =>
            {
                ProgramStatusBar.SetText("Загрузка файла...");
                XLWorkbook workbook = new(file);
                IXLWorksheet worksheet = workbook.Worksheet(1);
                
                foreach (KeyValuePair<string, string> pair in map)
                {
                    IXLCell colCell;
                    try
                    {
                        ProgramStatusBar.SetText($"Выполняется анализ для [{pair.Key}] ({pair.Value})...");
                        colCell = worksheet.Search(pair.Value, CompareOptions.IgnoreCase).First();   // ищем заголовок столбца с именем аналогичным тегу
                        int columnNumber = colCell.WorksheetColumn().ColumnNumber();    // номер столбца в листе Excel
                        int rowNumber = colCell.WorksheetRow().RowNumber() + 1; // номер строки в листе Excel
                        int fileIndex = 0; // индекс в List
                        for (int i = rowNumber; i <= worksheet.LastRowUsed().RowNumber(); i++)
                        {
                            if (result.Count < fileIndex + 1) // +1 поскольку счет индексов идет с нуля
                            {
                                result.Add([]);
                            }
                            string value = worksheet.Cell(i, columnNumber).Value.ToString();
                            result[fileIndex].Add(pair.Key, value);
                            fileIndex++;
                        }
                    }
                    catch (System.Exception)
                    {
                        continue;
                    }
                }
                ProgramStatusBar.Hide();
            });
            
            return result;
        }
    }
}
