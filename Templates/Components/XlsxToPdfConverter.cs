using ClosedXML.Excel;
using PdfSharp;
using PdfSharp.Pdf;

namespace Incas.Templates.Components
{
    public class XlsxToPdfConverter
    {
        public XLWorkbook workbook;
        public IXLWorksheet worksheet;
        private PdfDocument ResultedDocument;
        public XlsxToPdfConverter(string path)
        {
            this.ResultedDocument = new();
            this.workbook = new XLWorkbook(path);
            this.worksheet = this.workbook.Worksheet(1);
        }
        private void DefineRanges()
        {
            //foreach (IXLRange v in this.worksheet.PageSetup.PrintAreas)
            //{
            //    this.ResultedDocument.AddPage()
            //    this.ResultedDocument.
            //}
        }
    }
}
