using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;

namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        string XPSpath;
        XpsDocument document;
        public PreviewWindow(string path)
        {
            InitializeComponent();
            XPSpath = path;
            document = new XpsDocument(XPSpath, FileAccess.Read);
            this.Preview.Document = document.GetFixedDocumentSequence();
            document.Close();
        }

        private void OnClosed(object sender, EventArgs e)
        {
            File.Delete(XPSpath);
        }
    }
}
