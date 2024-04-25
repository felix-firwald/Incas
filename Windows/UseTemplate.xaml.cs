using ClosedXML.Excel;
using Common;
using Incubator_2.Common;
using Incubator_2.Forms;
using Incubator_2.Models;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Tag = Models.Tag;


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
            this.Title = t.name;
            this.CategoryName.Text = t.name;
            LoadTags();
            AddFileCreator();
            this.dir.Text = RegistryData.GetTemplatePreferredPath(this.template.id.ToString());
            ProgramState.ShowWaitCursor(false);
        }
        public UseTemplate(Template t, List<SGeneratedDocument> records)
        {
            InitializeComponent();
            this.template = t;
            this.Title = t.name;
            LoadTags();
            this.dir.Text = RegistryData.GetTemplatePreferredPath(this.template.id.ToString());
            this.CategoryName.Text = records[0].templateName;
            foreach (SGeneratedDocument item in records)
            {
                AddFileCreator().ApplyRecord(item);
            }
            ProgramState.ShowWaitCursor(false);
        }
        private void LoadTags()
        {
            using (Tag t = new())
            {
                tags = t.GetAllTagsByTemplate(this.template.id, this.template.parent);
            }
        }

        private UC_FileCreator AddFileCreator()
        {
            UC_FileCreator fc = new UC_FileCreator(template, tags);
            fc.OnInsertRequested += InsertValuesByTag;
            fc.OnRenameRequested += SimpleRecalculateNames;
            fc.OnCreatorDestroy += OnCreatorDestroy;
            this.ContentPanel.Children.Add(fc);
            creators.Add(fc);
            return fc;
        }

        private void OnCreatorDestroy(UC_FileCreator creator)
        {
            this.ContentPanel.Children.Remove(creator);
        }

        private void InsertValuesByTag(int tag, string value)
        {
            foreach (UC_FileCreator fc in creators)
            {
                fc.InsertTagValue(tag, value);
            }
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
                RegistryData.SetTemplatePreferredPath(this.template.id.ToString(), this.dir.Text);
                CreateFiles();
            }
        }
        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.ProgressCreation.Value = e.ProgressPercentage;
        }

        private void CreateFiles()
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            bool needSave = !this.template.GetTemplateSettings().PreventSave;
            foreach (UC_FileCreator fc in this.ContentPanel.Children)
            {
                if (!fc.CreateFile(this.dir.Text, this.CategoryName.Text, true, needSave))
                {
                    Mouse.OverrideCursor = null;
                    DatabaseManager.NullifyBackground();
                    return;
                }
            }
            DatabaseManager.ExecuteBackground();
            Mouse.OverrideCursor = null;
            this.Close();
            ProgramState.OpenFolder(this.dir.Text);
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
                if (tag.type != TypeOfTag.LocalConstant && tag.type != TypeOfTag.Text && tag.type != TypeOfTag.Table)
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
                    fc.RenameByTag(fr.SelectedTag, fr.Prefix, fr.Postfix, fr.IsAdditive);
                }
            }

        }
        private void SimpleRecalculateNames(string tag)
        {
            foreach (UC_FileCreator fc in this.creators)
            {
                fc.RenameByTag(tag, "", "");
            }
        }

        private void GetFromExcel(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "MS Excel|*.xlsx";

            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ProgramState.ShowWaitCursor();
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
                this.creators.Clear();
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
                foreach (Dictionary<string, string> item in pairs)
                {
                    var fc = AddFileCreator();
                    fc.ApplyFromExcel(item);
                }
                ProgramState.ShowWaitCursor(false);
            }
        }

        private void SendToExcel(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ValidateContent())
            {
                XLWorkbook wb = new XLWorkbook();
                IXLWorksheet ws = wb.AddWorksheet("Из инкубатора");
                for (int i = 0; i < tags.Count; i++) // columns
                {
                    IXLCell c = ws.Cell(1, i + 1).SetValue(tags[i].name);
                    c.Style.Font.Bold = true;
                }
                int row = 2;
                foreach (UC_FileCreator fc in this.ContentPanel.Children) // ряды
                {
                    int cell = 1;
                    foreach (string item in fc.GetExcelRow()) // ячейки в рядах
                    {
                        ws.Cell(row, cell).SetValue(item);
                        cell++;
                    }
                    row++;
                }
                try
                {
                    wb.SaveAs(this.dir.Text + $"\\{this.template.name} {DateTime.Now.ToString("d")}.xlsx");
                    ProgramState.OpenFolder(this.dir.Text);
                }
                catch (IOException)
                {
                    ProgramState.ShowErrorDialog("При попытке записать файл возникла ошибка,\nвозможно файл уже открыт." +
                        "\nЗакройте его и попробуйте снова", "Сохранение прервано");
                }

            }
        }

        private void SendToUserClick(object sender, MouseButtonEventArgs e)
        {
            List<SGeneratedDocument> documents = new();
            foreach (UC_FileCreator fc in this.ContentPanel.Children)
            {
                if (fc.SelectorChecked == true)
                {
                    documents.Add(fc.GetGeneratedDocument());
                }
            }
            if (documents.Count == 0)
            {
                ProgramState.ShowExclamationDialog("Не выбрано ни одного элемента для отправки! (используйте селекторы)", "Действие прервано");
            }
            else
            {
                Session session;
                if (ProgramState.ShowActiveUserSelector(out session, "Выберите пользователя для отправки формы."))
                {
                    ServerProcessor.SendOpenSequencerProcess(documents, session.slug);
                }

            }
        }
    }
}
