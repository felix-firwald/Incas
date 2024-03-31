using System;
using System.IO;
using System.Windows;
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
            try
            {
                File.Delete(XPSpath);
            }
            catch { }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}
