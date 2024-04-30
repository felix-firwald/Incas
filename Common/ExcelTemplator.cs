using ClosedXML.Excel;
using Incubator_2.Forms;
using Incubator_2.Models;
using Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xceed.Document.NET;

namespace Common
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
            Formatting head = new Formatting();
            head.Bold = true;
            head.FontFamily = new Font("Times New Roman");

            Formatting rowStyle = new Formatting();
            head.FontFamily = new Font("Times New Roman");
            this.worksheet.Row(cell.WorksheetRow().RowNumber() + 1).InsertRowsBelow(dt.Rows.Count);
            IXLTable it = cell.InsertTable(dt, true);
            it.Theme = new("Standart");
            it.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            it.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            this.workbook.Save();
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
