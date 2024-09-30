using ClosedXML.Excel;
using Incas.CreatedDocuments.Models;
using Incas.Templates.Views.Controls;
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
            IXLCell cell = this.worksheet.Search(this.ConvertTag(tag), System.Globalization.CompareOptions.IgnoreCase, false).FirstOrDefault();
            Formatting head = new()
            {
                Bold = true,
                FontFamily = new Font("Times New Roman")
            };

            Formatting rowStyle = new();
            head.FontFamily = new Font("Times New Roman");
            this.worksheet.Row(cell.WorksheetRow().RowNumber() + 1).InsertRowsBelow(dt.Rows.Count);
            IXLTable it = cell.InsertTable(dt, true);
            it.Theme = new("Standart");
            it.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            it.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            this.workbook.Save();
        }
        public void GenerateDocument(List<TagFiller> tagFillers, List<TableFiller> tableFillers, bool async = true)
        {
            List<string> tagsToReplace = new();
            List<string> values = new();
            foreach (TableFiller tab in tableFillers)
            {
                this.CreateTable(tab.field.Name, tab.DataTable);
            }
            foreach (TagFiller tf in tagFillers)
            {
                Guid id = tf.GetId();
                string name = tf.GetTagName();
                string value = tf.GetValue();
                tagsToReplace.Add(name);
                values.Add(value);
                if (tf.field.Type == TagType.Relation)
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
            List<string> result = new();
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
