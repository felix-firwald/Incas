using ClosedXML.Excel;
using Incas.Core.Classes;
using Incas.Objects.Interfaces;
using Incas.Objects.Views.Controls;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Incas.Templates.Components
{
    public class ExcelTemplator : ITemplator
    {
        private string Log = "";
        private List<string> tagsToReplace = [];
        private List<string> values = [];
        private Dictionary<string, DataTable> tables = new();
        public XLWorkbook workbook;
        public IXLWorksheet worksheet;
        public ExcelTemplator(string templatePath, string newPath)
        {
            string oldpath = ProgramState.GetFullnameOfDocumentFile(templatePath);
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
            if (dt.Rows.Count > 1)
            {
                this.worksheet.Row(rowIndex).InsertRowsBelow(dt.Rows.Count - 1);
            }
            foreach (DataRow dr in dt.Rows)
            {
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
        
        private void GetDataFromFillers(List<IFiller> fillers)
        {
            foreach (IFiller filler in fillers)
            {
                switch (filler.Field.Type)
                {
                    default:
                        FieldFiller ff = (FieldFiller)filler;
                        string value = ff.GetValue();
                        this.tagsToReplace.Add(filler.Field.Name);
                        this.values.Add(value);
                        if (ff.Field.Type == Objects.Components.FieldType.Relation)
                        {
                            foreach (KeyValuePair<string, string> fd in ff.GetDataFromObjectRelation())
                            {
                                this.tagsToReplace.Add(fd.Key);
                                this.values.Add(fd.Value);
                            }
                        }
                        break;
                    case Objects.Components.FieldType.Table:
                        this.tables.Add(filler.Field.Name, ((FieldTableFiller)filler).GetValue());
                        break;
                }
            }
        }
        public void GenerateDocument(List<IFiller> fillers)
        {
            this.GetDataFromFillers(fillers);
            foreach (KeyValuePair<string, DataTable> pair in this.tables)
            {
                this.CreateTable(pair.Key, pair.Value);
            }
            this.Replace(this.tagsToReplace, this.values);
        }
        public async void GenerateDocumentAsync(List<IFiller> fillers)
        {
            this.GetDataFromFillers(fillers);
            await System.Threading.Tasks.Task.Run(() =>
            {
                foreach (KeyValuePair<string, DataTable> pair in this.tables)
                {
                    this.CreateTable(pair.Key, pair.Value);
                }
                this.Replace(this.tagsToReplace, this.values);
            });
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
            return result;
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
    }
}
