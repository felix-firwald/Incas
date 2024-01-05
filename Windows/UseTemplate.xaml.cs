using ClosedXML.Excel;
using Common;
using Incubator_2.Common;
using Incubator_2.Forms;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;


namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для UseTemplate.xaml
    /// </summary>
    public partial class UseTemplate : Window
    {
        Template template;
        List<Tag> tags;
        List<UC_FileCreator> creators = new List<UC_FileCreator>();
        delegate void UpdateProgressBarDelegate(DependencyProperty dp, object value);
        private readonly BackgroundWorker worker = new BackgroundWorker();
        public UseTemplate(Template t)
        {
            InitializeComponent();
            this.template = t;
            LoadTags();
            AddFileCreator();
            this.dir.Text = RegistryData.GetTemplatePreferredPath(this.template.id.ToString());
        }
        public UseTemplate(Template t, List<TemplateJSON> records)
        {
            InitializeComponent();
            this.template = t;
            LoadTags();
            this.dir.Text = RegistryData.GetTemplatePreferredPath(this.template.id.ToString());
            foreach (TemplateJSON item in records)
            {
                AddFileCreator().ApplyRecord(item);
            }
        }
        private void LoadTags()
        {
            using (Tag t = new())
            {
                tags = t.GetAllTagsByTemplate(template.id, template.parent);
            } 
        }

        private UC_FileCreator AddFileCreator()
        {
            UC_FileCreator fc = new UC_FileCreator(template, tags);
            this.ContentPanel.Children.Add(fc);
            creators.Add(fc);
            return fc;
        }

        private bool ValidateContent()
        {
            if (!Directory.Exists(this.dir.Text))
            {
                ProgramState.ShowErrorDialog($"Папка, указанная в качестве выходной для генерации документов ({this.dir.Text}) не существует.\nУкажите существующую!", "Несуществующий выходной путь");
                return false;
            }
            return true;
        }

        private void CreateFilesClick(object sender, RoutedEventArgs e)
        {
            if (ValidateContent())
            {
                worker.DoWork += CreateFiles;
                //worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                worker.ProgressChanged += OnProgressChanged;
                worker.WorkerReportsProgress = true;
                worker.RunWorkerAsync();
                //CreateFiles();
            }
        }
        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.ProgressCreation.Value = e.ProgressPercentage;
        }

        private void CreateFiles(object sender, DoWorkEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                RegistreCreatedJSON.GetRegistry();
                int i = 1;
                foreach (UC_FileCreator fc in creators)
                {
                    fc.CreateFile(this.dir.Text);
                    int perc = i * 100 / creators.Count;
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        worker.ReportProgress(perc);
                    });
                    
                    i++;
                }
                RegistreCreatedJSON.SaveRegistry();      
            
                System.Diagnostics.Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", this.dir.Text);
            });
        }

        private void AddFC_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AddFileCreator();
        }

        private void review_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.RootFolder = System.Environment.SpecialFolder.MyDocuments;
            if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.dir.Text = fb.SelectedPath;
            }
        }
        private void MinimizeAll(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (UC_FileCreator fc in this.ContentPanel.Children)
            {
                fc.Minimize();
            }
        }

        private void MaximizeAll(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (UC_FileCreator fc in this.ContentPanel.Children)
            {
                fc.Maximize();
            }
        }
        private void RecalculateNamesClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            List<string> names = new List<string>();
            foreach (Tag tag in tags)
            {
                if (tag.type != TypeOfTag.LocalConstant && tag.type != TypeOfTag.Text)
                {
                    names.Add(tag.name);
                }
            }
            FilenamesRecalculator fr = new FilenamesRecalculator(this.template.id, names);
            fr.ShowDialog();
            if (fr.status == DialogStatus.Yes)
            {
                foreach (UC_FileCreator fc in this.creators)
                {
                    fc.RenameByTag(fr.SelectedTag, fr.Prefix, fr.Postfix);
                }
            }
        }

        private void GetFromExcel(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "MS Excel|*.xlsx";
            
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                List<Dictionary<string, string>> pairs = new();    // список "файлов", где каждый элемент в списке - это список тегов и значений относящихся к файлу
                IXLWorksheet ws;
                try
                {
                    XLWorkbook wb = new XLWorkbook(fd.FileName);
                    ws = wb.Worksheet(1);
                }
                catch (IOException)
                {
                    ProgramState.ShowErrorDialog("Файл занят другим процессом. Его использование невозможно.");
                    return;
                }
                this.ContentPanel.Children.Clear();
                foreach (Tag tag in tags)
                {
                    IXLCell colCell;
                    try
                    {
                        colCell = ws.Search(tag.name, CompareOptions.IgnoreCase).First();   // ищем заголовок столбца с именем аналогичным тегу
                        int columnNumber = colCell.WorksheetColumn().ColumnNumber();    // номер столбца в листе Excel
                        int rowNumber = colCell.WorksheetRow().RowNumber() + 1; // номер строки в листе Excel
                        int fileIndex = 0; // индекс в List
                        for (int i = rowNumber; i <= ws.LastRowUsed().RowNumber(); i++)
                        {
                            if (pairs.Count < fileIndex + 1) // +1 поскольку счет индексов идет с нуля
                            {
                                pairs.Add(new());
                            }
                            string value = ws.Cell(i, columnNumber).Value.ToString();
                            pairs[fileIndex].Add(tag.name, value);
                            fileIndex++;
                        }
                        
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                foreach (Dictionary<string,string> item in pairs)
                {
                    var fc = AddFileCreator();
                    fc.ApplyFromExcel(item);
                }
            }
        }

        private void SendToExcel(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
