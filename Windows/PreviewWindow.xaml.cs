using Common;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
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
        private bool printEnabled;
        public PreviewWindow(string path, bool printEnabled = true)
        {
            InitializeComponent();
            XPSpath = path;
            document = new XpsDocument(XPSpath, FileAccess.Read);
            this.Preview.Document = document.GetFixedDocumentSequence();
            document.Close();
            this.printEnabled = printEnabled;
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

        private void CommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (!this.printEnabled)
            {
                ProgramState.ShowExclamationDialog("Этот тип документа требует сохранения в историю, для создания файла используйте кнопку \"Создать файлы по шаблону\".", "Печать прервана");
                return;
            }
            System.Windows.Controls.PrintDialog printDialog = new();
            bool? isPrinted = printDialog.ShowDialog();
            if (isPrinted != true)
            {
                return;
            }                
            try
            {
                // Open the selected document.
                XpsDocument xpsDocument = new(XPSpath, FileAccess.Read);
                
                // Get a fixed document sequence for the selected document.
                FixedDocumentSequence fixedDocSeq = xpsDocument.GetFixedDocumentSequence();

                // Create a paginator for all pages in the selected document.
                DocumentPaginator docPaginator = fixedDocSeq.DocumentPaginator;

                // Print to a new file.
                printDialog.PrintDocument(docPaginator, $"Печать документа {Path.GetFileName(XPSpath)}");
            }
            catch (Exception)
            {

            }
        }
    }
}
