using ClosedXML.Excel;
using Incas.Core.Classes;
using Incas.Objects.AutoUI;
using Incas.Objects.Components;
using Incas.Objects.Exceptions;
using Incas.Objects.Models;
using Incas.Objects.Views.Pages;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Incas.Objects.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для ObjectsEditor.xaml
    /// </summary>
    public partial class ObjectsEditor : Window
    {
        public readonly Class Class;
        public readonly ClassData ClassData;
        public delegate void UpdateRequested();
        public delegate void CreateRequested(Guid id);
        public event UpdateRequested OnUpdateRequested;
        public event CreateRequested OnSetNewObjectRequested;
        public ObjectsEditor(Class source, List<Components.Object> objects = null)
        {
            this.InitializeComponent();
            this.Title = source.name;
            this.Class = source;
            this.ClassData = source.GetClassData();
            if (this.ClassData.ClassType == ClassType.Document)
            {
                this.RenderButton.Visibility = Visibility.Visible;
            }
            else
            {
                this.RenderButton.Visibility = Visibility.Collapsed;
            }
            if (objects != null)
            {
                foreach (Components.Object obj in objects)
                {
                    this.AddObjectCreator(obj);
                }
            }
            else
            {
                this.AddObjectCreator();
            }
        }
        public ObjectsEditor(Class source, ClassData data) // dev
        {
            this.InitializeComponent();
            this.Title = "Режим предпросмотра: " + source.name;
            this.Class = source;
            this.ClassData = data;
            this.GenerateButton.IsEnabled = false;
            this.RenderButton.IsEnabled = false;
            this.ToolBar.IsEnabled = false;
            ObjectCreator creator = new(this.ClassData);
            this.ContentPanel.Children.Add(creator);
            if (this.ClassData.ClassType == ClassType.Document)
            {
                this.RenderButton.Visibility = Visibility.Visible;
            }
            else
            {
                this.RenderButton.Visibility = Visibility.Collapsed;
            }
        }
        public void SetSingleObjectMode()
        {
            this.GenerateButton.IsEnabled = false;
            this.ToolBar.IsEnabled = false;
        }

        private ObjectCreator AddObjectCreator(Components.Object obj = null)
        {
            ObjectCreator creator = new(this.Class, this.ClassData, obj);
            creator.OnSaveRequested += this.Creator_OnSaveRequested;
            creator.OnRemoveRequested += this.Creator_OnRemoveRequested;
            creator.OnInsertRequested += this.Creator_OnInsertRequested;
            this.ContentPanel.Children.Add(creator);
            return creator;
        }

        private void Creator_OnInsertRequested(Guid id, string text)
        {
            foreach (ObjectCreator c in this.ContentPanel.Children)
            {
                c.InsertToField(id, text);
            }
        }

        private void Creator_OnRemoveRequested(ObjectCreator creator)
        {
            this.ContentPanel.Children.Remove(creator);
        }

        private void Creator_OnSaveRequested(ObjectCreator creator)
        {
            try
            {
                ObjectProcessor.WriteObjects(this.Class, creator.PullObject());
                this.OnUpdateRequested?.Invoke();
                this.OnSetNewObjectRequested?.Invoke(creator.Object.Id);
            }
            catch (NotNullFailed nnex)
            {
                DialogsManager.ShowExclamationDialog(nnex.Message, "Сохранение прервано");
            }
            catch (AuthorFailed af)
            {
                DialogsManager.ShowAccessErrorDialog(af.Message);
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }
        }

        private void AddClick(object sender, MouseButtonEventArgs e)
        {
            this.AddObjectCreator();
        }

        private void GetFromExcel(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog fd = new()
            {
                Filter = "MS Excel|*.xlsx"
            };

            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DialogsManager.ShowWaitCursor();
                List<Dictionary<string, string>> pairs = [];    // список "файлов", где каждый элемент в списке - это список тегов и значений относящихся к файлу
                IXLWorksheet ws;
                try
                {
                    XLWorkbook wb = new(fd.FileName);
                    ws = wb.Worksheet(1);
                }
                catch (IOException)
                {
                    DialogsManager.ShowErrorDialog("Файл занят другим процессом. Его использование невозможно.");
                    return;
                }
                this.ContentPanel.Children.Clear();
                foreach (Objects.Models.Field tag in this.ClassData.Fields)
                {
                    IXLCell colCell;
                    try
                    {
                        colCell = ws.Search(tag.Name, CompareOptions.IgnoreCase).First();   // ищем заголовок столбца с именем аналогичным тегу
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
                            pairs[fileIndex].Add(tag.Name, value);
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
                    ObjectCreator fc = this.AddObjectCreator();
                    fc.ApplyFromExcel(item);
                }
                DialogsManager.ShowWaitCursor(false);
            }
        }
        private void SendToExcel(object sender, MouseButtonEventArgs e)
        {
            XLWorkbook wb = new();
            try
            {
                IXLWorksheet ws = wb.AddWorksheet("Из INCAS");
                for (int i = 0; i < this.ClassData.Fields.Count; i++) // columns
                {
                    IXLCell c = ws.Cell(1, i + 1).SetValue(this.ClassData.Fields[i].Name);
                    c.Style.Font.Bold = true;
                }
                int row = 2;
                foreach (ObjectCreator fc in this.ContentPanel.Children) // ряды
                {
                    int cell = 1;
                    foreach (string item in fc.GetExcelRow()) // ячейки в рядах
                    {
                        ws.Cell(row, cell).SetValue(item);
                        cell++;
                    }
                    row++;
                }
            
                string path = "";
                if (DialogsManager.ShowFolderBrowserDialog(ref path))
                {
                    wb.SaveAs(path + $"\\{this.Class.name} {DateTime.Now.ToString("d")}.xlsx");
                    ProgramState.OpenFolder(path);
                }               
            }
            catch (NotNullFailed nnex)
            {
                DialogsManager.ShowExclamationDialog(nnex.Message, "Сохранение прервано");
            }
            catch (AuthorFailed af)
            {
                DialogsManager.ShowAccessErrorDialog(af.Message);
            }
            catch (IOException)
            {
                DialogsManager.ShowErrorDialog("При попытке записать файл возникла ошибка,\nвозможно файл уже открыт." +
                    "\nЗакройте его и попробуйте снова", "Сохранение прервано");
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex, "Сохранение прервано");
            }
        }
        private void MinimizeAll(object sender, MouseButtonEventArgs e)
        {
            foreach (ObjectCreator c in this.ContentPanel.Children)
            {
                c.Minimize();
            }
        }

        private void MaximizeAll(object sender, MouseButtonEventArgs e)
        {
            foreach (ObjectCreator c in this.ContentPanel.Children)
            {
                c.Maximize();
            }
        }

        private void CreateObjectsClick(object sender, RoutedEventArgs e)
        {
            List<Components.Object> objects = new();
            try
            {
                foreach (ObjectCreator c in this.ContentPanel.Children)
                {
                    objects.Add(c.PullObject());
                }
                ObjectProcessor.WriteObjects(this.Class, objects);
                this.Close();
                this.OnUpdateRequested?.Invoke();
            }            
            catch (NotNullFailed nnex)
            {
                DialogsManager.ShowExclamationDialog(nnex.Message, "Сохранение прервано");
            }
            catch (AuthorFailed af)
            {
                DialogsManager.ShowAccessErrorDialog(af.Message);
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }
        }

        private void RenderObjectsClick(object sender, RoutedEventArgs e)
        {
            List<Components.Object> objects = new();
            string templateFile = "";
            if (this.ClassData.Templates?.Count == 1)
            {
                templateFile = this.ClassData.Templates[1].File;
            }
            else if (this.ClassData.Templates?.Count > 1)
            {
                TemplateSelection ts = new(this.ClassData);
                if (ts.ShowDialog("Выбор шаблона", Incas.Core.Classes.Icon.Magic) == true)
                {
                    templateFile = ts.GetSelectedPath();
                }
            }
            else
            {
                DialogsManager.ShowExclamationDialog("Не найдены привязанные шаблоны к этому классу.", "Рендеринг невозможен");
                return;
            }
            string path = "";
            if (DialogsManager.ShowFolderBrowserDialog(ref path) == true)
            {
                try
                {
                    foreach (ObjectCreator c in this.ContentPanel.Children)
                    {
                        objects.Add(c.PullObject());
                        c.GenerateDocument(templateFile, path);
                    }
                    ObjectProcessor.WriteObjects(this.Class, objects);
                    this.OnUpdateRequested?.Invoke();
                    ProgramState.OpenFolder(path);
                }
                catch (NotNullFailed nnex)
                {
                    DialogsManager.ShowExclamationDialog(nnex.Message, "Рендеринг прерван");
                }
                catch (AuthorFailed af)
                {
                    DialogsManager.ShowAccessErrorDialog(af.Message);
                }
                catch (Exception ex)
                {
                    DialogsManager.ShowErrorDialog(ex);
                }
            }          
        }
    }
}
