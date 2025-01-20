using ClosedXML.Excel;
using Incas.Core.Classes;
using Incas.Objects.AutoUI;
using Incas.Objects.Components;
using Incas.Objects.Exceptions;
using Incas.Objects.Models;
using Incas.Objects.Views.Pages;
using Incas.Templates.AutoUI;
using Incas.Templates.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public readonly Preset Preset;
        public delegate void UpdateRequested();
        public delegate void CreateRequested(Guid id);
        public event UpdateRequested OnUpdateRequested;
        public event CreateRequested OnSetNewObjectRequested;
        public ObjectsEditor(Class source, Preset preset, List<Components.Object> objects = null)
        {
            this.InitializeComponent();
            this.Title = preset is null ? source.name : $"{source.name} — {preset.Name}";
            this.Class = source;         
            this.ClassData = source.GetClassData();
            this.RenderButton.Visibility = this.ClassData.ClassType == ClassType.Document ? Visibility.Visible : Visibility.Collapsed;
            this.Preset = preset;
            this.FillPresettingData();
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
            DialogsManager.ShowWaitCursor(false);
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
            this.RenderButton.Visibility = this.ClassData.ClassType == ClassType.Document ? Visibility.Visible : Visibility.Collapsed;
            DialogsManager.ShowWaitCursor(false);
        }
        public void SetSingleObjectMode()
        {
            this.GenerateButton.IsEnabled = false;
            this.ToolBar.IsEnabled = false;
        }
        public void FillPresettingData()
        {
            if (this.Preset is not null)
            {
                foreach (Models.Field f in this.ClassData.Fields)
                {
                    string value;
                    if (this.Preset.Data.TryGetValue(f.Id, out value))
                    {
                        this.Preset.RegisterPresettingField(f, value);
                    }
                }
            }
        }
        private ObjectCreator AddObjectCreator(Components.Object obj = null)
        {
            if (obj is not null)
            {
                if ((this.Preset is null && obj.Preset != Guid.Empty) // в едиторе нет пресета а у объекта есть
                    || (this.Preset is not null && obj.Preset == Guid.Empty) // в едиторе есть пресет а у объекта нет
                    || (this.Preset is not null && obj.Preset != this.Preset.Id )) // в едиторе есть пресет и он отличается от того что в объекте
                {
                    DialogsManager.ShowExclamationDialog($"Объект с наименованием \"{obj.Name}\" не будет добавлен в коллекцию редактирования, поскольку пресет, к которому он привязан, отличается от пресета первого объекта в списке.", "Редактирование ограничено");
                    return null;
                }             
            }
            ObjectCreator creator = new(this.Class, this.Preset, obj);
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

        private bool Creator_OnRemoveRequested(ObjectCreator creator)
        {
            this.ContentPanel.Children.Remove(creator);
            return true;
        }

        private bool Creator_OnSaveRequested(ObjectCreator creator)
        {
            try
            {
                ObjectProcessor.WriteObjects(this.Class, creator.PullObject());
                this.OnUpdateRequested?.Invoke();
                if (this.OnSetNewObjectRequested != null)
                {
                    this.OnSetNewObjectRequested?.Invoke(creator.Object.Id);
                }            
                return true;
            }
            catch (NotNullFailed nnex)
            {
                DialogsManager.ShowExclamationDialog(nnex.Message, "Сохранение прервано");
                return false;
            }
            catch (AuthorFailed af)
            {
                DialogsManager.ShowAccessErrorDialog(af.Message);
                return false;
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
                return false;
            }
        }

        private void AddClick(object sender, MouseButtonEventArgs e)
        {
            this.AddObjectCreator();
        }

        private async void GetFromExcel(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog fd = new()
            {
                Filter = "MS Excel|*.xlsx"
            };

            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DialogsManager.ShowWaitCursor();               
                ExcelImporterSettings ei = new(this.ClassData.Fields);
                if (ei.ShowDialog("Настройка импорта", Core.Classes.Icon.Table))
                {
                    this.ContentPanel.Children.Clear();
                    try
                    {
                        List<Dictionary<string, string>> pairs = await ExcelTemplator.GetFromFile(fd.FileName, ei.GetMap());
                        foreach (Dictionary<string, string> item in pairs)
                        {
                            ObjectCreator fc = this.AddObjectCreator();
                            fc.ApplyFromExcel(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        DialogsManager.ShowErrorDialog(ex);
                    }
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

        private async void CreateObjectsClick(object sender, RoutedEventArgs e)
        {
            List<Components.Object> objects = [];
            try
            {
                foreach (ObjectCreator c in this.ContentPanel.Children)
                {
                    objects.Add(c.PullObject());
                }
                this.Close();
                bool result = await ObjectProcessor.WriteObjects(this.Class, objects);
                if (result)
                {
                    this.OnUpdateRequested?.Invoke();
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
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }
        }

        private async void RenderObjectsClick(object sender, RoutedEventArgs e)
        {
            List<Components.Object> objects = [];
            TemplateData templateFile = new();
            if (this.ClassData.Templates?.Count == 1)
            {
                templateFile = this.ClassData.Templates[1];
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
            string path = RegistryData.GetClassTemplatePrefferedPath(this.Class.identifier, templateFile.Name);
            if (DialogsManager.ShowFolderBrowserDialog(ref path) == true)
            {
                try
                {
                    RegistryData.SetClassTemplatePrefferedPath(this.Class.identifier, templateFile.Name, path);
                    ProgramStatusBar.SetText("Выполняется рендеринг объектов...");
                    foreach (ObjectCreator c in this.ContentPanel.Children)
                    {                      
                        objects.Add(c.PullObject());
                        await c.GenerateDocument(templateFile, path);
                    }
                    ProgramStatusBar.Hide();
                    bool result = await ObjectProcessor.WriteObjects(this.Class, objects);
                    if (result)
                    {
                        this.OnUpdateRequested?.Invoke();
                    }              
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
