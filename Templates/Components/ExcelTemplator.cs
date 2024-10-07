using ClosedXML.Excel;
using Incas.Objects.Views.Controls;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using WebSupergoo.WordGlue3;
using Xceed.Document.NET;

namespace Incas.Templates.Components
{
    public class ExcelTemplator : ITemplator
    {
        public XLWorkbook workbook;
        public IXLWorksheet worksheet;
        public ExcelTemplator(string path)
        {
            this.workbook = new XLWorkbook(path);
            this.worksheet = this.workbook.Worksheet(1);
        }
        private string ConvertTag(string tag)
        {
            return $"[{tag}]";
        }
        public async void Replace(List<string> tags, List<string> values, bool async = true)
        {
            void MakeReplace()
            {
                for (int i = 0; i < tags.Count; i++)
                {
                    IXLCells c = this.worksheet.Search(this.ConvertTag(tags[i]));
                    foreach (IXLCell item in c)
                    {
                        item.Value = values[i];
                    }
                }
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
            this.workbook.Save();
        }
        public void CreateTable(string tag, DataTable dt)
        {
            int rowIndex = -1;
            int lastColumnIndex = -1;
            int firstColumnIndex = -1;
            Dictionary<string, IXLCell> columns = new();
            IXLCell cell = this.worksheet.Search($"[{tag}.", System.Globalization.CompareOptions.IgnoreCase, false).FirstOrDefault();
            if (cell is null)
            {
                return;
            }
            rowIndex = cell.WorksheetRow().RowNumber();
            foreach (DataColumn dc in dt.Columns)
            {
                IXLCell columnCell = this.worksheet.Search($"[{tag}.{dc.ColumnName}]", System.Globalization.CompareOptions.IgnoreCase, false).FirstOrDefault();
                if (cell is not null)
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
            if (dt.Rows.Count > 1)
            {
                this.worksheet.Row(rowIndex).InsertRowsBelow(dt.Rows.Count - 1);
            }
            foreach (DataRow dr in dt.Rows)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    string value = dr[dc.ColumnName].ToString();
                    int currentCol = columns[dc.ColumnName].WorksheetColumn().ColumnNumber();
                    this.worksheet.Cell(rowIndex, currentCol).SetValue(value);
                    this.worksheet.Cell(rowIndex, currentCol).Style = columns[dc.ColumnName].Style;
                    IXLRangeAddress range = columns[dc.ColumnName].MergedRange().RangeAddress;
                    if (range.ColumnSpan > 1)
                    {
                        this.worksheet.Range(rowIndex, currentCol, rowIndex, currentCol + range.ColumnSpan - 1).Merge();
                    }
                }
                rowIndex += 1;
            }
            this.workbook.Save();
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
    }
}
