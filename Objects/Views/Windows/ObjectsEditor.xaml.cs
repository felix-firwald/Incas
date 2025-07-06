using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using Incas.Core.Classes;
using Incas.Core.Views.Controls;
using Incas.Miniservices.UserStatistics;
using Incas.Objects.AutoUI;
using Incas.Objects.Interfaces;
using Incas.Objects.Views.Controls;
using Incas.Objects.Views.Pages;
using Incas.Rendering.AutoUI;
using Incas.Rendering.Components;
using IncasEngine.Core.Registry;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Exceptions;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Groups.Components;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using static Incas.Objects.Interfaces.IObjectEditorForm;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Incas.Objects.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для ObjectsEditor.xaml
    /// </summary>
    public partial class ObjectsEditor : System.Windows.Window, IObjectEditorForm
    {
        public readonly IClass Class;
        public readonly IClassData ClassData;
        
        public delegate void CreateRequested(Guid id);
        public event UpdateRequested OnUpdateRequested;
        public GroupClassPermissionSettings PermissionSettings { get; set; }
        public event CreateRequested OnSetNewObjectRequested;
        public ObjectsEditor(IClass source, List<IObject> objects = null)
        {
            this.InitializeComponent();
            this.Title = source.Name;
            this.Class = source;           
            if (!ProgramState.CurrentWorkspace.CurrentGroup.IsComponentInAccess(source.Component))
            {
                this.ShowDisabledComponentMessage();
            }
            else
            {
                this.ClassData = source.GetClassData();
                this.PermissionSettings = ProgramState.CurrentWorkspace.CurrentGroup.GetClassPermissions(source.Id);
                this.RenderButton.Visibility = source.Type == ClassType.Document ? Visibility.Visible : Visibility.Collapsed;
                if (objects != null)
                {
                    foreach (IObject obj in objects)
                    {
                        this.AddObjectCreator(obj);
                    }
                }
                else
                {
                    this.AddObjectCreator();

                }                
                this.Class.OnUpdated += this.EngineEvents_OnUpdateClassRequested;
                this.Class.OnRemoved += this.Class_OnRemoved;
            }
            DialogsManager.ShowWaitCursor(false);
        }
        public ObjectsEditor(IClass source, ClassData data) // dev
        {
            this.InitializeComponent();
            this.Title = "Режим предпросмотра: " + source.Name;
            this.Class = source;
            this.ClassData = data;
            this.GenerateButton.IsEnabled = false;
            this.RenderButton.IsEnabled = false;
            this.ToolBar.IsEnabled = false;
            ObjectCreator creator = new(this.ClassData);
            this.ContentPanel.Children.Add(creator);
            this.RenderButton.Visibility = source.Type == ClassType.Document ? Visibility.Visible : Visibility.Collapsed;
            DialogsManager.ShowWaitCursor(false);
        }
        private void Class_OnRemoved()
        {
            this.Close();
        }
        private void SetOtherContent(UIElement element)
        {
            this.MainGrid.Children.Clear();
            this.MainGrid.Children.Add(element);
            Grid.SetRow(element, 0);
            Grid.SetRowSpan(element, 2);
            Grid.SetColumnSpan(element, 2);
            Grid.SetColumn(element, 0);
        }
        private void EngineEvents_OnUpdateClassRequested()
        {
            this.Dispatcher.Invoke(() =>
            {
                ClassUpdatedMessage message = new();
                this.SetOtherContent(message);
            });            
        }
        private void ShowDisabledComponentMessage()
        {
            ComponentNotActive na = new();
            this.SetOtherContent(na);
        }

        public void SetSingleObjectMode()
        {
            this.GenerateButton.IsEnabled = false;
            this.ToolBar.IsEnabled = false;
        }
        private ObjectCreator AddObjectCreator(IObject obj = null)
        {
            ObjectCreator creator = new(this.Class, obj);
            creator.PermissionSettings = this.PermissionSettings;
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

        private async Task<bool> Creator_OnSaveRequested(ObjectCreator creator)
        {
            bool result = false;
            try
            {
                IObject obj = creator.PullObject();
                result = await Processor.WriteObjects(this.Class, obj);
                this.OnUpdateRequested?.Invoke();
                if (this.OnSetNewObjectRequested != null)
                {
                    this.OnSetNewObjectRequested?.Invoke(creator.Object.Id);
                }            
                return true;
            }
            catch (FieldDataFailed nnex)
            {
                DialogsManager.ShowExclamationDialog(nnex.Message, "Сохранение прервано");
                return result;
            }
            catch (NotNullFailed nnex)
            {
                DialogsManager.ShowExclamationDialog(nnex.Message, "Сохранение прервано");
                return result;
            }
            catch (AuthorFailed af)
            {
                DialogsManager.ShowAccessErrorDialog(af.Message);
                return result;
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
                return result;
            }
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            this.AddObjectCreator();
        }

        private async void GetFromExcel(object sender, RoutedEventArgs e)
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
        private void SendToExcel(object sender, RoutedEventArgs e)
        {
            XLWorkbook wb = new();
            try
            {
                IXLWorksheet ws = wb.AddWorksheet(this.ClassData.ListName);
                for (int i = 0; i < this.ClassData.Fields.Count; i++) // columns
                {
                    IXLCell c = ws.Cell(1, i + 1).SetValue(this.ClassData.Fields[i].VisibleName);
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
                    wb.SaveAs(path + $"\\{this.ClassData.ListName} {DateTime.Now.ToString("d")}.xlsx");
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
        private void MinimizeAll(object sender, RoutedEventArgs e)
        {
            foreach (ObjectCreator c in this.ContentPanel.Children)
            {
                c.Minimize();
            }
        }

        private void MaximizeAll(object sender, RoutedEventArgs e)
        {
            foreach (ObjectCreator c in this.ContentPanel.Children)
            {
                c.Maximize();
            }
        }

        private async void CreateObjectsClick(object sender, RoutedEventArgs e)
        {
            List<IObject> objects = [];
            try
            {
                foreach (ObjectCreator c in this.ContentPanel.Children)
                {
                    objects.Add(c.PullObject());
                }
                this.Hide();
                bool result = await Processor.WriteObjects(this.Class, objects);
                if (result)
                {
                    this.OnUpdateRequested?.Invoke();
                    StatisticsManager.AddWorkedObjects(objects);
                }
                //this.Show();
                
                this.Close();
            }
            catch (FieldDataFailed fdf)
            {
                this.Show();
                DialogsManager.ShowExclamationDialog(fdf.Message, "Сохранение прервано");
            }
            catch (NotNullFailed nnex)
            {
                this.Show();
                DialogsManager.ShowExclamationDialog(nnex.Message, "Сохранение прервано");
            }
            catch (AuthorFailed af)
            {
                this.Show();
                DialogsManager.ShowAccessErrorDialog(af.Message);
            }
            catch (Exception ex)
            {
                this.Show();
                DialogsManager.ShowErrorDialog(ex);
            }
        }

        private async void RenderObjectsClick(object sender, RoutedEventArgs e)
        {
            List<IObject> objects = [];
            IncasEngine.ObjectiveEngine.Types.Documents.Document doc = (this.ContentPanel.Children[0] as ObjectCreator).PullObject() as IncasEngine.ObjectiveEngine.Types.Documents.Document;
            
            Template templateFile = new();
            DocumentClassData docData = this.ClassData as DocumentClassData;
            if (docData.Documents?.Count == 1)
            {
                templateFile = docData.Documents[0];
            }
            else if (docData.Documents?.Count > 1)
            {
                TemplateSelection ts = new(docData);
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
            //string path = RegistryData.GetClassTemplatePrefferedPath(this.Class.Id, templateFile);
            string path = "";
            if (DialogsManager.ShowFolderBrowserDialog(ref path) == true)
            {
                try
                {
                    //RegistryData.SetClassTemplatePrefferedPath(this.Class.Id, templateFile, path);
                    ProgramStatusBar.SetText("Выполняется рендеринг объектов...");
                    foreach (ObjectCreator c in this.ContentPanel.Children)
                    {                      
                        objects.Add(c.PullObject());
                        await c.GenerateDocument(templateFile, path, false);
                    }
                    ProgramStatusBar.Hide();
                    bool result = await Processor.WriteObjects(this.Class, objects);
                    if (result)
                    {
                        this.OnUpdateRequested?.Invoke();
                    }              
                    ProgramState.OpenFolder(path);                    
                }
                catch (FieldDataFailed fdf)
                {
                    DialogsManager.ShowExclamationDialog(fdf.Message, "Рендеринг прерван");
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

        #region Static
        private static Dictionary<IClass, ObjectsEditor> editorsOpened = new();
        public static ObjectsEditor Open(IClass source, List<IObject> objects)
        {
            if (editorsOpened.ContainsKey(source) && PresentationSource.FromVisual(editorsOpened[source]) != null)
            {
                ObjectsEditor targetEditor = editorsOpened[source];
                targetEditor.MinimizeAll(null, null);
                foreach (IObject obj in objects)
                {
                    targetEditor.AddObjectCreator(obj);
                }
                DialogsManager.ShowWaitCursor(false);
                return targetEditor;
            }
            else
            {
                ObjectsEditor oe = new(source, objects);
                editorsOpened[source] = oe;
                oe.Show();
                DialogsManager.ShowWaitCursor(false);
                return oe;
            }          
        }
        #endregion
    }
}
