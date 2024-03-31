using ClosedXML.Excel;

namespace Common
{
    public class ExcelTemplator
    {
        public XLWorkbook book;
        public IXLWorksheet worksheet;
        public ExcelTemplator(string path)
        {
            book = new XLWorkbook(path);
            worksheet = book.Worksheet(1);
        }
        public void Replace()
        {

        }
    }
}
