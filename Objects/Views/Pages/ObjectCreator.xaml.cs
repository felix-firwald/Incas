using Incas.Core.Classes;
using Incas.Core.Interfaces;
using Incas.Objects.AutoUI;
using Incas.Objects.Components;
using Incas.Objects.Interfaces;
using Incas.Objects.Views.Controls;
using Incas.Objects.Views.Windows;
using Incas.Rendering.Components;
using IncasEngine.Core.ExtensionMethods;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Exceptions;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.Documents;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Groups.Components;
using IncasEngine.Scripting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static System.Windows.Forms.AxHost;

namespace Incas.Objects.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ObjectCreator.xaml
    /// </summary>
    public partial class ObjectCreator : System.Windows.Controls.UserControl, ICollapsible
    {
        /// <summary>
        /// Target object
        /// </summary>
        public IObject Object { get; set; }

        /// <summary>
        /// Target class
        /// </summary>
        public IClass Class { get; set; }

        /// <summary>
        /// Parents classes of target class
        /// </summary>
        private List<IClass> parents { get; set; }

        /// <summary>
        /// ClassData of target class
        /// </summary>
        public IClassData ClassData { get; set; }
        public delegate Task<bool> ObjectCreatorDataAsync(ObjectCreator creator);
        public delegate bool ObjectCreatorData(ObjectCreator creator);
        public delegate void FieldCopyAction(Guid id, string text);
        public GroupClassPermissionSettings PermissionSettings { get; set; }
        public event FieldCopyAction OnInsertRequested;
        public event ObjectCreatorData OnUpdated;
        public event ObjectCreatorDataAsync OnSaveRequested;
        public event ObjectCreatorData OnRemoveRequested;
        private bool Locked = false;
        private Dictionary<Field, IFillerBase> fillers;
        private IServiceFieldFiller serviceFiller;
        private Dictionary<Button, Method> buttons;

        public ObjectCreator(IClass source, IObject obj = null)
        {
            this.InitializeComponent();
            this.Class = source;
            this.ClassData = source.GetClassData();
            this.parents = source.GetParentClasses();
            this.Object = obj;           
            if (obj != null)
            {
                this.FillContentPanel();
                this.ApplyObject(obj, true);
            }
            else
            {
                this.Object = Helpers.CreateObjectByType(source);
                this.FillContentPanel();             
            }
            if (this.Class.Type != ClassType.Document)
            {
                this.RenderArea.Visibility = Visibility.Collapsed;
            }           
            else
            {
                this.ApplyTerminated();
            }
            this.ExpanderButton.IsChecked = true;
            this.ApplyAuthorConstraint();
        }
        public ObjectCreator(IClassData data) // dev mode
        {
            this.InitializeComponent();
            this.ClassData = data;
            this.FillContentPanel();
            this.Object = Helpers.CreateObjectByType(new Class());
            this.DocumentTools.Visibility = Visibility.Collapsed;
            this.RemoveButton.Visibility = Visibility.Collapsed;
        }
        private void ApplyTerminated()
        {
            if (Helpers.IsObjectTerminated(this.Object))
            {
                this.RenderArea.Visibility = Visibility.Collapsed;
                this.SaveArea.Visibility = Visibility.Collapsed;
                this.TerminatedIcon.Visibility = Visibility.Visible;
                this.ContentPanel.IsEnabled = false;
                SolidColorBrush color = new(System.Windows.Media.Color.FromRgb(52, 201, 36));
                this.Separator.Fill = color;
            }
        }
        private void ApplyAuthorConstraint()
        {
            if (this.Object.Id != Guid.Empty && this.ClassData.EditByAuthorOnly == true && !Helpers.CheckAuthor(this.Object))
            {
                this.Locked = true;
                this.ContentPanel.IsEnabled = false;
                System.Windows.Controls.Label label = new()
                {
                    Content = "Вы не можете редактировать этот объект, поскольку не являетесь его автором.",
                    Margin = new Thickness(5),
                    Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0))
                };
                this.ContentPanel.Children.Insert(0, label);
            }
        }
        private void FillContentPanel()
        {
            FormDrawingManager.DrawingOutputArgs args = FormDrawingManager.Start().DrawForm(this.Object, this.ContentPanel);

            this.fillers = args.Fillers;
            this.serviceFiller = args.ServiceFiller;
            this.buttons = args.Buttons;
            foreach (KeyValuePair<Field, IFillerBase> ff in this.fillers)
            {
                ff.Value.OnInsert += this.Tf_OnInsert;
                ff.Value.OnFillerUpdate += this.Tf_OnFieldUpdate;
                ff.Value.OnDatabaseObjectCopyRequested += this.Tf_OnDatabaseObjectCopyRequested;
            }
            foreach (KeyValuePair<Button, Method> pair in args.Buttons)
            {
                pair.Key.Click += this.ButtonWithMethodClicked;
            }
        }

        private void ButtonWithMethodClicked(object sender, RoutedEventArgs e)
        {
            Method method = this.buttons[(Button)sender];
            IObject obj = this.PullObjectForScript();
            CodeOutputArgs output = obj.RunMethod(method);
            this.ApplyObject(obj, output.StateUpdated);
        }
        private void ApplyState()
        {
            foreach (IncasEngine.ObjectiveEngine.Models.State state in this.ClassData.States)
            {
                if (state.Id == this.Object.State)
                {
                    foreach (KeyValuePair<Field, IFillerBase> filler in this.fillers)
                    {
                        filler.Value.ApplyState(state);
                    }
                    break;
                }
            }
        }

        private void Tf_OnDatabaseObjectCopyRequested(IFillerBase sender)
        {
            BindingData bd = new()
            {
                BindingClass = this.Class.Id,
                BindingField = sender.Field.Id
            };
            DatabaseSelection ds = new(bd);
            ds.ShowDialog();
            IObject obj = ds.GetSelectedObject();
            if (obj is null)
            {
                return;
            }
            foreach (FieldData field in obj.Fields)
            {
                if (field.ClassField.Id == sender.Field.Id)
                {
                    sender.SetValue(field.Value.ToString());
                    return;
                }
            }
        }

        private void Tf_OnFieldUpdate(IFillerBase sender)
        {
            this.OnUpdated?.Invoke(this);
        }
        public void ApplyFromExcel(Dictionary<string, string> pairs)
        {
            foreach (KeyValuePair<string, string> pair in pairs)
            {
                foreach (IFillerBase tf in this.ContentPanel.Children)
                {
                    if (tf.Field.Name == pair.Key)
                    {
                        tf.SetValue(pair.Value);
                        break;
                    }
                }
            }
        }
        public void ApplyObject(IObject obj, bool updateState)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            this.ObjectName.Text = obj.Name;
            foreach (FieldData data in obj.Fields)
            {
                this.fillers[data.ClassField].SetValue(data.Value.ToString());
                //foreach (IFillerBase filler in this.fillers)
                //{
                //    if (filler.Field.Id == data.ClassField.Id)
                //    {                     
                //        filler.SetValue();         
                //        break;
                //    }
                //}
            }
            stopwatch.Stop();
            DialogsManager.ShowInfoDialog(stopwatch.GetTextResult());
            if (updateState)
            {
                this.ApplyState();
            }          
        }
        public IObject PullObjectForScript()
        {
            if (this.Object.Fields == null)
            {
                this.Object.Fields = [];
            }
            this.Object.Fields.Clear();
            if (this.serviceFiller is not null)
            {
                this.Object = this.serviceFiller.GetResult();
            }
            foreach (KeyValuePair<Field,IFillerBase> tf in this.fillers)
            {
                FieldData data = new()
                {
                    ClassField = tf.Key,
                    Value = tf.Value.GetDataForScript()
                };
                this.Object.Fields.Add(data);
            }
            return this.Object;
        }
        public IObject PullObject()
        {
            if (this.Locked == true)
            {
                throw new AuthorFailed($"Объект с именем \"{this.Object.Name}\" не может быть модифицирован, поскольку не вы являетесь его автором.");
            }
            if (this.Object.Id == Guid.Empty && !this.PermissionSettings.CreateOperations) // if new
            {
                throw new AuthorFailed($"Вы не можете создавать объекты этого класса.");
            }
            else if (this.Object.Id != Guid.Empty && !this.PermissionSettings.UpdateOperations)// if already exists
            {
                throw new AuthorFailed($"Вы не можете редактировать объекты этого класса.");
            }
            this.UpdateName();
            this.Object.Name = this.ObjectName.Text;
            if (this.Object.Fields == null)
            {
                this.Object.Fields = [];
            }
            this.Object.Fields.Clear();
            if (this.serviceFiller is not null)
            {
                this.Object = this.serviceFiller.GetResult();
            }
            foreach (KeyValuePair<Field,IFillerBase> tf in this.fillers)
            {
                FieldData data = new()
                {
                    ClassField = tf.Key,
                    Value = tf.Value.GetData()
                };
                this.Object.Fields.Add(data);
            }
            return this.Object;
        }
        private string RemoveUnresolvedChars(string input)
        {
            return input
                .Replace("/", "")
                .Replace("\\", "")
                .Replace(":", "")
                .Replace("?", "")
                .Replace("*", "")
                .Replace("<", "")
                .Replace(">", "")
                .Replace("|", "")
                .Replace("\"", "")
                .Trim();
        }
        public string GetNameOfFile(string folder, string template, string extension)
        {
            string result = "";
            string templatePart = "";
            if (((DocumentClassData)this.ClassData).InsertTemplateName)
            {
                templatePart = " (" + template + ")";
            }
            result = $"{folder}\\{this.RemoveUnresolvedChars(this.ObjectName.Text)}{templatePart}.{extension}";
            return result;
        }
        public async void GenerateDocument()
        {
            string name = this.ObjectName.Text;
            string path = "";
            DocumentClassData docData = this.ClassData as DocumentClassData;
            if (docData.Documents?.Count == 1)
            {
                if (DialogsManager.ShowFolderBrowserDialog(ref path) == true)
                {
                    await this.GenerateDocument(docData.Documents[1], path, true);
                }
            }
            else if (docData.Documents?.Count > 1)
            {
                TemplateSelection ts = new(docData);
                if (ts.ShowDialog("Выбор шаблона", Icon.Magic) == true)
                {
                    if (DialogsManager.ShowFolderBrowserDialog(ref path) == true)
                    {
                        await this.GenerateDocument(ts.GetSelectedPath(), path, true);
                    }
                }
            }
            else
            {

            }
        }
        public async Task<string> GenerateDocument(Template templateData, string folder, bool needsPullObject)
        {
            string newFile = "";
            string oldFile = templateData.File;
            ITemplator templ = null;           
            try
            {
                if (needsPullObject)
                {
                    this.PullObject();
                }
                
                if (oldFile.EndsWith(".docx"))
                {
                    newFile = this.GetNameOfFile(folder, templateData.Name, "docx");
                    WordTemplator wt = new(templateData, newFile);
                    templ = wt;                
                }
                else if (oldFile.EndsWith(".xlsx"))
                {
                    newFile = this.GetNameOfFile(folder, templateData.Name, "xlsx");
                    ExcelTemplator et = new(templateData, newFile);
                    templ = et;                  
                }
                this.Dispatcher.Invoke(() =>
                {
                    
                });
                bool result = await templ.GenerateDocumentAsync(this.Object as Document);
                return newFile;
            }
            catch (IOException ioex)
            {
                DialogsManager.ShowErrorDialog($"При доступе к файлу \"{this.ObjectName.Text}\" или его папке возникла ошибка.\n" +
                    $"Возможно существует файл с таким же именем, который уже открыт другим пользователем.\nПодробности: " + ioex.Message + "\n" +
                    $"Файл будет пропущен.");
                return "";
            }
            catch (Exception e)
            {
                DialogsManager.ShowErrorDialog($"При рендеринге документа возникла неизвестная ошибка: {e.Message}.");
                return "";
            }
        }
        private string UpdateName()
        {
            string name = "";
            if (this.ClassData.NameTemplate is not null)
            {
                name = this.ClassData.NameTemplate;
                foreach (KeyValuePair<Field,IFillerBase> tf in this.fillers)
                {
                    switch (tf.Key.Type)
                    {
                        case FieldType.Table:
                            break;
                        default:
                            ISimpleFiller simple = (ISimpleFiller)tf.Value;
                            string val = simple.GetValue();
                            if (val != null)
                            {
                                name = name.Replace("[" + simple.GetTagName() + "]", val);
                            }
                            break;
                    }
                }
            }
            else
            {
                return this.ObjectName.Text;
            }
           
            this.ObjectName.Text = name;
            return name;
        }

        private void Tf_OnInsert(Guid tag, string text)
        {
            this.OnInsertRequested?.Invoke(tag, text);
        }
        public void InsertToField(Guid id, string data)
        {
            foreach (KeyValuePair<Field,IFillerBase> tf in this.fillers)
            {
                if (tf.Key.Id == id)
                {
                    tf.Value.SetValue(data);
                    return;
                }
            }
        }

        public void Maximize()
        {
            this.ExpanderButton.IsChecked = true;
        }
        public void Minimize()
        {
            this.ExpanderButton.IsChecked = false;
        }
        private void MaximizeClick(object sender, RoutedEventArgs e)
        {
            this.MainBorder.Height = this.ContentPanel.Height + 40;
        }
        private void MinimizeClick(object sender, RoutedEventArgs e)
        {
            this.MainBorder.Height = 40;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            this.OnSaveRequested?.Invoke(this);
        }

        private async void PreviewCLick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (await this.OnSaveRequested?.Invoke(this) == false)
                {
                    return;
                }
                DocumentClassData docData = this.ClassData as DocumentClassData;
                string path = ProgramState.CurrentWorkspace.GetRuntimesTemplatesFolder();
                if (docData.Documents?.Count == 1)
                {
                    if (docData.Documents[0].File.EndsWith(".xlsx"))
                    {
                        DialogsManager.ShowExclamationDialog("Предпросмотр для Excel файлов недоступен.", "Рендеринг прерван");
                        return;
                    }
                    ProgramStatusBar.SetText("Рендеринг документа...");
                    DialogsManager.ShowWaitCursor(true);
                    string name = await this.GenerateDocument(docData.Documents[0], path, true);
                    DialogsManager.ShowWebViewer($"Предварительный просмотр ({this.Class.Name})", WordTemplator.ReplaceToPDF(name), true);

                }
                else if (docData.Documents?.Count > 1)
                {
                    TemplateSelection ts = new(docData);
                    if (ts.ShowDialog("Выбор шаблона", Icon.Magic) == true)
                    {
                        if (ts.GetSelectedPath().File.EndsWith(".xlsx"))
                        {
                            DialogsManager.ShowExclamationDialog("Предпросмотр для Excel файлов недоступен.", "Рендеринг прерван");
                            return;
                        }
                        ProgramStatusBar.SetText("Рендеринг документа...");
                        string name = await this.GenerateDocument(ts.GetSelectedPath(), path, true);
                        DialogsManager.ShowWebViewer($"Предварительный просмотр ({this.Class.Name})", WordTemplator.ReplaceToPDF(name), true);
                    }
                }
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }
        }
        public List<string> GetExcelRow()
        {
            List<string> output = [];
            foreach (KeyValuePair<Field,IFillerBase> tf in this.fillers)
            {
                switch (tf.Key.Type)
                {
                    case FieldType.Table:
                        break;
                    default:
                        output.Add(((ISimpleFiller)tf.Value).GetValue());
                        break;                   
                }             
            }
            return output;
        }

        private void RemoveClick(object sender, RoutedEventArgs e)
        {
            this.OnRemoveRequested?.Invoke(this);
        }
    }
}
