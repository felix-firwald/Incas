using Incas.Core.Classes;
using Incas.Core.Interfaces;
using Incas.Objects.AutoUI;
using Incas.Objects.Components;
using Incas.Objects.Interfaces;
using Incas.Objects.Views.Controls;
using Incas.Objects.Views.Windows;
using Incas.Rendering.Components;
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
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static IncasEngine.ObjectiveEngine.Models.State;

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
        public delegate void ObjectState(State state);
        public GroupClassPermissionSettings PermissionSettings { get; set; }
        public event FieldCopyAction OnInsertRequested;
        public event ObjectCreatorData OnUpdated;
        public event ObjectState OnStateUpdated;
        public event ObjectCreatorDataAsync OnSaveRequested;
        public event ObjectCreatorData OnRemoveRequested;
        private bool Locked = false;
        private Dictionary<Field, IFillerBase> fillers;
        private IServiceFieldFiller serviceFiller;
        private Dictionary<Button, Method> buttons;
        public Dictionary<Table, FieldTableFiller> tables;

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
                this.ApplyObject(this.Object, true);
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
        public void HideNCA()
        {
            this.DocumentTools.Visibility = Visibility.Collapsed;
            this.RemoveButtonRect.Visibility = Visibility.Collapsed;
            this.RemoveButton.Visibility = Visibility.Collapsed;
            this.Separator.Visibility = Visibility.Collapsed;
            Grid.SetRow(this.ContentPanel, 0);
            Grid.SetRowSpan(this.ContentPanel, 3);
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
            this.tables = args.Tables;
            foreach (KeyValuePair<Field, IFillerBase> ff in this.fillers)
            {
                ff.Value.OnInsert += this.Tf_OnInsert;
                ff.Value.OnFillerUpdate += this.Tf_OnFieldUpdate;
                ff.Value.OnDatabaseObjectCopyRequested += this.Tf_OnDatabaseObjectCopyRequested;
                if (ff.Value is ITableFiller tableFiller)
                {
                    tableFiller.OnCustomButtonClicked += this.ButtonWithMethodClicked;
                }
            }
            foreach (KeyValuePair<Button, Method> pair in args.Buttons)
            {
                pair.Key.Click += this.ButtonWithMethodClicked;
            }
        }
        public void ApplyMethod(Method method)
        {
            try
            {
                DialogsManager.ShowWaitCursor();
                IObject obj = this.PullObjectForScript();
                CodeOutputArgs output = obj.RunMethod(method);
                this.ApplyObject(obj, output.StateUpdated);
                if (method.AutoSave)
                {
                    this.OnSaveRequested?.Invoke(this);
                }
                DialogsManager.ShowWaitCursor(false);
            }            
            catch (AccessException)
            {
                DialogsManager.ShowAccessErrorDialog("У вас нет доступа на вызов методов этого класса.");
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }
        }
        private void ButtonWithMethodClicked(object sender, RoutedEventArgs e)
        {           
            Method method = this.buttons[(Button)sender];
            this.ApplyMethod(method);
        }
        private void ApplyState()
        {
            if (this.ClassData.States is null)
            {
                return;
            }
            try
            {                
                foreach (IncasEngine.ObjectiveEngine.Models.State state in this.ClassData.States)
                {
                    if (state.Id == this.Object.State)
                    {
                        this.OnStateUpdated?.Invoke(state);
                        foreach (KeyValuePair<Field, IFillerBase> filler in this.fillers)
                        {
                            filler.Value.ApplyState(state);
                        }
                        foreach (KeyValuePair<Table, FieldTableFiller> filler in this.tables)
                        {
                            filler.Value.ApplyState(state);
                        }                       
                        foreach (KeyValuePair<Button, Method> button in this.buttons)
                        {
                            try
                            {
                                MemberState memberState = state.Settings[button.Value.Id];
                                button.Key.IsEnabled = memberState.IsEnabled;
                                button.Key.Visibility = memberState.EditorVisibility == true ? Visibility.Visible : Visibility.Collapsed;
                            }
                            catch
                            {

                            }
                        }                        
                        return;
                    }
                }
                this.SetUndefinedState();
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }
        }
        public void SetUndefinedState()
        {
            this.PullObjectForScript();
            StateUndefinedMessage sum = new(this.Object);
            this.MainGrid.Children.Clear();
            this.MainGrid.Children.Add(sum);
            Grid.SetRowSpan(sum, 3);
            Grid.SetColumnSpan(sum, 3);
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

        private void Tf_OnFieldUpdate(Guid sender)
        {
            if (sender == Guid.Empty)
            {
                return;
            }
            foreach (Method m in this.ClassData.Methods)
            {
                if (m.Id == sender)
                {
                    this.ApplyMethod(m);
                }              
            }
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
            this.ObjectName.Text = obj.Name;
            foreach (FieldData data in obj.Fields)
            {
                if (data.Value is not null)
                {
                    this.fillers[data.ClassField].SetValue(data.Value?.ToString());
                }
            }
            if (obj.Tables is not null)
            {
                foreach (KeyValuePair<Table, DataTable> dt in obj.Tables)
                {
                    this.tables[dt.Key].SetData(dt.Value);
                }
            }           
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
            if (this.Object.Fields == null)
            {
                this.Object.Fields = [];
            }
            this.Object.Fields.Clear();
            if (this.serviceFiller is not null)
            {
                this.Object = this.serviceFiller.GetResult();
            }
            this.Object.Name = this.ClassData.NameTemplate;
            foreach (KeyValuePair<Field,IFillerBase> tf in this.fillers)
            {
                string value = tf.Value.GetData();
                FieldData data = new()
                {
                    ClassField = tf.Key,
                    Value = value
                };
                if (tf.Key.Type == FieldType.Object)
                {
                    string replacement = ((FieldFiller)tf.Value).GetValue();
                    this.Object.Name = this.Object.Name.Replace("[" + tf.Key.Name + "]", replacement);
                }
                else
                {
                    this.Object.Name = this.Object.Name.Replace("[" + tf.Key.Name + "]", value);
                }
                
                this.Object.Fields.Add(data);
            }
            this.ObjectName.Text = this.Object.Name;
            if (this.Object.Tables is not null)
            {
                this.Object.Tables.Clear();
                foreach (KeyValuePair<Table, FieldTableFiller> tablePair in this.tables)
                {
                    this.Object.Tables.Add(tablePair.Key, tablePair.Value.GetValue());
                }
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
                output.Add(((ISimpleFiller)tf.Value).GetValue());
                break;                                
            }
            return output;
        }

        private void RemoveClick(object sender, RoutedEventArgs e)
        {
            this.OnRemoveRequested?.Invoke(this);
        }
    }
}
