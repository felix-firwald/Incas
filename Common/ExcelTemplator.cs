using ClosedXML.Excel;
using Incas.CreatedDocuments.Models;
using Incas.Templates.Views.Controls;
using IncasEngine.TemplateManager;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xceed.Document.NET;

namespace Incas.Common
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
        public List<SGeneratedTag> GenerateDocument(List<TagFiller> tagFillers, List<TableFiller> tableFillers, string number, bool isAsync = true)
        {
            List<SGeneratedTag> filledTags = [];
            List<string> tagsToReplace = ["N"];
            List<string> values = [number];
            foreach (TableFiller tab in tableFillers)
            {
                this.CreateTable(tab.tag.name, tab.DataTable);
                filledTags.Add(tab.GetAsGeneratedTag());
            }
            foreach (TagFiller tf in tagFillers)
            {
                int id = tf.GetId();
                string name = tf.GetTagName();
                string value = tf.GetValue();
                if (tf.tag.type != TagType.LocalConstant)
                {
                    if (tf.tag.type is TagType.Generator or TagType.Date)
                    {
                        SGeneratedTag gtg = new()
                        {
                            tag = id,
                            value = tf.GetData()
                        };
                        filledTags.Add(gtg);
                    }
                    else
                    {
                        SGeneratedTag gt = new()
                        {
                            tag = id,
                            value = value
                        };
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
