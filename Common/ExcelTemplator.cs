using ClosedXML.Excel;
using Incubator_2.Forms;
using Incubator_2.Models;
using Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace Common
{
    public class ExcelTemplator : ITemplator
    {
        public XLWorkbook workbook;
        public IXLWorksheet worksheet;
        public ExcelTemplator(string path)
        {
            workbook = new XLWorkbook(path);
            worksheet = workbook.Worksheet(1);
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
                    IXLCells c = worksheet.Search(ConvertTag(tags[i]));
                    foreach (var item in c)
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
            workbook.Save();
        }
        public void CreateTable(string tag, DataTable dt)
        {
            IXLCell cell = worksheet.Search(ConvertTag(tag), System.Globalization.CompareOptions.IgnoreCase, false).FirstOrDefault();
            Formatting head = new Formatting();
            head.Bold = true;
            head.FontFamily = new Font("Times New Roman");

            Formatting rowStyle = new Formatting();
            head.FontFamily = new Font("Times New Roman");
            for (int i = 0; i < dt.Columns.Count; i++) // cols
            {
                IXLCell cursor = worksheet.Cell(cell.WorksheetRow().RowNumber(), cell.WorksheetColumn().ColumnNumber());
                for (int row = 0; row < dt.Rows.Count; row++) // rows
                {
                    cell.WorksheetRow().InsertRowsBelow(row);
                    //tab.Rows[row + 1].Cells[i].Paragraphs[0].Append(dt.Rows[row][i].ToString(), rowStyle);
                }
            }
            

            workbook.Save();
        }
        public List<SGeneratedTag> GenerateDocument(List<UC_TagFiller> tagFillers, List<TableFiller> tableFillers, string number, bool isAsync = true)
        {
            List<SGeneratedTag> filledTags = new();
            List<string> tagsToReplace = new List<string> { "N" };
            List<string> values = new List<string> { number };
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
                if (tf.tag.type != TypeOfTag.LocalConstant)
                {
                    if (tf.tag.type == TypeOfTag.Generator || tf.tag.type == TypeOfTag.Date)
                    {
                        SGeneratedTag gtg = new();
                        gtg.tag = id;
                        gtg.value = tf.GetData();
                        filledTags.Add(gtg);
                    }
                    else
                    {
                        SGeneratedTag gt = new();
                        gt.tag = id;
                        gt.value = value;
                        filledTags.Add(gt);
                    }
                }
                tagsToReplace.Add(name);
                values.Add(value);
            }
            this.Replace(tagsToReplace, values, isAsync);
            return filledTags;
        }
    }
}
