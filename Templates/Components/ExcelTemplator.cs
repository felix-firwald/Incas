using ClosedXML.Excel;
using Incas.Objects.Views.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
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
