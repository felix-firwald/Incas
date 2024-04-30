using Common;
using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;

namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        private string XPSpath;
        private XpsDocument document;
        private bool printEnabled;
        public PreviewWindow(string path, bool printEnabled = true)
        {
            InitializeComponent();
            this.XPSpath = path;
            this.document = new XpsDocument(this.XPSpath, FileAccess.Read);
            this.Preview.Document = this.document.GetFixedDocumentSequence();
            this.document.Close();
            this.printEnabled = printEnabled;
        }

        private void OnClosed(object sender, EventArgs e)
        {
            try
            {
                File.Delete(this.XPSpath);
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
                XpsDocument xpsDocument = new(this.XPSpath, FileAccess.Read);

                // Get a fixed document sequence for the selected document.
                FixedDocumentSequence fixedDocSeq = xpsDocument.GetFixedDocumentSequence();

                // Create a paginator for all pages in the selected document.
                DocumentPaginator docPaginator = fixedDocSeq.DocumentPaginator;

                // Print to a new file.
                printDialog.PrintDocument(docPaginator, $"Печать документа {Path.GetFileName(this.XPSpath)}");
            }
            catch (Exception)
            {

            }
        }
    }
}
