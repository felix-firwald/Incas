using ClosedXML.Excel;
using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Incas.CreatedDocuments.Models;
using Incas.Templates.Components;
using Incas.Templates.Models;
using Incas.Templates.Views.Pages;
using Incas.Users.Models;
using IncasEngine.TemplateManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Tag = Incas.Templates.Models.Tag;

namespace Incas.Templates.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для UseTemplate.xaml
    /// </summary>
    public partial class UseTemplate : Window
    {
        private Template template;
        private TemplateSettings templateSettings;
        private List<Tag> tags;
        private List<FileCreator> creators = [];

        private delegate void UpdateProgressBarDelegate(DependencyProperty dp, object value);
        private readonly BackgroundWorker worker = new();
        private void Setup(Template t)
        {
            this.InitializeComponent();
            this.template = t;
            this.Title = t.name;
            this.CategoryName.Text = t.name;
            this.templateSettings = t.GetTemplateSettings();
            this.LoadTags();
            this.dir.Text = RegistryData.GetTemplatePreferredPath(this.template.id.ToString());
            ProgramState.ShowWaitCursor(false);
        }
        public UseTemplate(Template t)
        {
            this.Setup(t);
            this.AddFileCreator();
        }
        public UseTemplate(Template t, List<SGeneratedDocument> records)
        {
            this.Setup(t);
            this.CategoryName.Text = records[0].templateName;
            foreach (SGeneratedDocument item in records)
            {
                this.AddFileCreator().ApplyRecord(item);
            }
            ProgramState.ShowWaitCursor(false);
        }
        public UseTemplate(string templateName, List<Tag> tags) // dev mode
        {
            this.InitializeComponent();
            this.tags = tags;
            this.Title = "Режим предпросмотра: " + templateName;
            this.CategoryName.IsEnabled = false;
            this.ToolBar.IsEnabled = false;
            this.GenerateButton.IsEnabled = false;
            this.dir.IsEnabled = false;
            this.ReviewButton.IsEnabled = false;
            this.AddFileCreatorDev();
        }
        private void LoadTags()
        {
            using Tag t = new();
            this.tags = t.GetAllTagsByTemplate(this.template.id, this.template.parent);
        }

        private FileCreator AddFileCreator()
        {
            FileCreator fc = new(this.template, ref this.templateSettings, this.tags);
            fc.OnInsertRequested += this.InsertValuesByTag;
            fc.OnRenameRequested += this.SimpleRecalculateNames;
            fc.OnCreatorDestroy += this.OnCreatorDestroy;
            this.ContentPanel.Children.Add(fc);
            this.creators.Add(fc);
            return fc;
        }
        private FileCreator AddFileCreatorDev()
        {
            FileCreator fc = new(this.tags);
            fc.OnInsertRequested += this.InsertValuesByTag;
            fc.OnRenameRequested += this.SimpleRecalculateNames;
            fc.OnCreatorDestroy += this.OnCreatorDestroy;
            this.ContentPanel.Children.Add(fc);
            this.creators.Add(fc);
            return fc;
        }

        private void OnCreatorDestroy(FileCreator creator)
        {
            this.ContentPanel.Children.Remove(creator);
        }

        private void InsertValuesByTag(int tag, string value)
        {
            foreach (FileCreator fc in this.creators)
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
            if (this.ValidateContent())
            {
                RegistryData.SetTemplatePreferredPath(this.template.id.ToString(), this.dir.Text);
                this.CreateFiles();
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
            foreach (FileCreator fc in this.ContentPanel.Children)
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
            this.AddFileCreator();
        }

        private void review_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fb = new()
            {
                RootFolder = System.Environment.SpecialFolder.MyDocuments
            };
            if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.dir.Text = fb.SelectedPath;
            }
        }
        private void MinimizeAll(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (FileCreator fc in this.ContentPanel.Children)
            {
                fc.Minimize();
            }
        }

        private void MaximizeAll(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (FileCreator fc in this.ContentPanel.Children)
            {
                fc.Maximize();
            }
        }
        private void RecalculateNamesClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            List<string> names = [];
            foreach (Tag tag in this.tags)
            {
                if (tag.type is not TagType.LocalConstant and not TagType.Text and not TagType.Table)
                {
                    names.Add(tag.name);
                }
            }
            FilenamesRecalculator fr = new(this.template.id, names);
            fr.ShowDialog();
            if (fr.status == DialogStatus.Yes)
            {
                foreach (FileCreator fc in this.creators)
                {
                    fc.RenameByTag(fr.SelectedTag, fr.Prefix, fr.Postfix, fr.IsAdditive);
                }
            }
        }
        private void SimpleRecalculateNames(string tag)
        {
            foreach (FileCreator fc in this.creators)
            {
                fc.RenameByTag(tag, "", "");
            }
        }

        private void GetFromExcel(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenFileDialog fd = new()
            {
                Filter = "MS Excel|*.xlsx"
            };

            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ProgramState.ShowWaitCursor();
                List<Dictionary<string, string>> pairs = [];    // список "файлов", где каждый элемент в списке - это список тегов и значений относящихся к файлу
                IXLWorksheet ws;
                try
                {
                    XLWorkbook wb = new(fd.FileName);
                    ws = wb.Worksheet(1);
                }
                catch (IOException)
                {
                    ProgramState.ShowErrorDialog("Файл занят другим процессом. Его использование невозможно.");
                    return;
                }
                this.ContentPanel.Children.Clear();
                this.creators.Clear();
                foreach (Tag tag in this.tags)
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
                                pairs.Add([]);
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
                    FileCreator fc = this.AddFileCreator();
                    fc.ApplyFromExcel(item);
                }
                ProgramState.ShowWaitCursor(false);
            }
        }

        private void SendToExcel(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.ValidateContent())
            {
                XLWorkbook wb = new();
                IXLWorksheet ws = wb.AddWorksheet("Из инкубатора");
                for (int i = 0; i < this.tags.Count; i++) // columns
                {
                    IXLCell c = ws.Cell(1, i + 1).SetValue(this.tags[i].name);
                    c.Style.Font.Bold = true;
                }
                int row = 2;
                foreach (FileCreator fc in this.ContentPanel.Children) // ряды
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
            List<SGeneratedDocument> documents = [];
            foreach (FileCreator fc in this.ContentPanel.Children)
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
