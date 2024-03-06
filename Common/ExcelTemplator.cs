using System;
using ClosedXML;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
