using Incas.Core.Classes;
using Incas.Core.Interfaces;
using Incas.Objects.AutoUI;
using Incas.Objects.Interfaces;
using Incas.Objects.Views.Controls;
using Incas.Objects.Views.Windows;
using Incas.Rendering.Components;
using IncasEngine.Backups;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Exceptions;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.Documents;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Groups.Components;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Incas.Objects.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ObjectCreator.xaml
    /// </summary>
    public partial class ObjectCreator : System.Windows.Controls.UserControl, ICollapsible
    {
        public IObject Object { get; set; }
        public IClass Class { get; set; }
        public IClassData ClassData { get; set; }
        public delegate bool ObjectCreatorData(ObjectCreator creator);
        public delegate void FieldCopyAction(Guid id, string text);
        public GroupClassPermissionSettings PermissionSettings { get; set; }
        public event FieldCopyAction OnInsertRequested;
        public event ObjectCreatorData OnUpdated;
        public event ObjectCreatorData OnSaveRequested;
        public event ObjectCreatorData OnRemoveRequested;
        private bool Locked = false;
        private List<IFillerBase> fillers;
        private IServiceFieldFiller serviceFiller;
        public ObjectCreator(IClass source, IObject obj = null)
        {
            this.InitializeComponent();
            this.Class = source;
            this.ClassData = source.GetClassData();
            this.Object = obj;           
            if (obj != null)
            {
                this.FillContentPanel();
                this.ApplyObject(obj);
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
            this.fillers = new();
            this.serviceFiller = Components.ServiceExtensionFieldsManager.GetFillerByType(this.Object);
            if (this.serviceFiller != null)
            {
                this.ContentPanel.Children.Add((UserControl)this.serviceFiller);
            }
            foreach (Field f in this.ClassData.Fields)
            {
                switch (f.Type)
                {
                    default:
                        FieldFiller ff = new(f)
                        {
                            Uid = f.Id.ToString()
                        };
                        ff.OnInsert += this.Tf_OnInsert;
                        ff.OnFillerUpdate += this.Tf_OnFieldUpdate;
                        ff.OnScriptRequested += this.Ff_OnScriptRequested;
                        ff.OnDatabaseObjectCopyRequested += this.Tf_OnDatabaseObjectCopyRequested;
                        this.ContentPanel.Children.Add(ff);
                        this.fillers.Add(ff);
                        break;
                    case FieldType.Table:
                        FieldTableFiller ft = new(f)
                        {
                            Uid = f.Id.ToString()
                        };
                        ft.OnInsert += this.Tf_OnInsert;
                        ft.OnFillerUpdate += this.Tf_OnFieldUpdate;
                        ft.OnDatabaseObjectCopyRequested += this.Tf_OnDatabaseObjectCopyRequested;
                        this.ContentPanel.Children.Add(ft);
                        this.fillers.Add(ft);
                        break;
                    case FieldType.Generator:
                        FieldGeneratorFiller fg = new(f)
                        {
                            Uid = f.Id.ToString()
                        };
                        fg.OnInsert += this.Tf_OnInsert;
                        fg.OnFillerUpdate += this.Tf_OnFieldUpdate;
                        fg.OnDatabaseObjectCopyRequested += this.Tf_OnDatabaseObjectCopyRequested;
                        this.ContentPanel.Children.Add(fg);
                        this.fillers.Add(fg);
                        break;
                }
            }         
        }

        private async void Ff_OnScriptRequested(string script)
        {
            try
            {
                Dictionary<Field, object> dict = this.PullObjectForScript();
                await Task.Run(() =>
                {
                    string scriptResult = this.ClassData.Script;
                    ScriptEngine engine = Python.CreateEngine();
                    ScriptScope scope = engine.CreateScope();
                    scriptResult += $"\nmain = {this.Class.Name.Replace(" ", "_")}(";
                    List<string> args = new();
                    foreach (KeyValuePair<Field, object> fd in dict)
                    {
                        switch (fd.Key.Type)
                        {
                            case FieldType.Number:
                                args.Add($"{fd.Key.Name}={fd.Value}");
                                break;
                            case FieldType.Date:
                                DateTime dt = (DateTime)fd.Value;
                                args.Add($"{fd.Key.Name}=datetime.date({dt.Year}, {dt.Month}, {dt.Day})");
                                break;
                            default:
                                args.Add($"{fd.Key.Name}='''{fd.Value}'''");
                                break;
                        }                       
                    }
                    scriptResult += string.Join(", ", args);
                    scriptResult += $")\nmain.{script}()";
                    engine.Execute(scriptResult, scope);

                    List<FieldData> fields = new();
                    dynamic target = scope.GetVariable("main");
                    foreach (KeyValuePair<Field, object> fd in dict)
                    {
                        fields.Add(new()
                        {
                            ClassField = fd.Key,
                            Value = engine.Operations.GetMember(target, fd.Key.Name).ToString()
                        });
                    }
                    this.Object.Fields = fields;
                });
                this.ApplyObject(this.Object);
            }
            catch(Exception ex)
            {
                DialogsManager.ShowErrorDialog("При обработке скрипта возникла ошибка:\n" + ex.Message, "Ошибка скрипта");
            }
        }

        private void Tf_OnDatabaseObjectCopyRequested(IFillerBase sender)
        {
            BindingData bd = new()
            {
                Class = this.Class.Id,
                Field = sender.Field.Id
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
                    sender.SetValue(field.Value);
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
        public void ApplyObject(IObject obj)
        {
            this.ObjectName.Text = obj.Name;
            foreach (FieldData data in obj.Fields)
            {
                foreach (IFillerBase filler in this.fillers)
                {
                    if (filler.Field.Id == data.ClassField.Id)
                    {                     
                        filler.SetValue(data.Value);         
                        break;
                    }
                }
            }      
        }
        public Dictionary<Field, object> PullObjectForScript()
        {
            if (this.Object.Fields == null)
            {
                this.Object.Fields = [];
            }
            this.Object.Fields.Clear();
            Dictionary<Field, object> pairs = new();
            foreach (IFillerBase tf in this.ContentPanel.Children)
            {
                pairs.Add(tf.Field, tf.GetDataForScript());
            }
            return pairs;
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
            foreach (IFillerBase tf in this.fillers)
            {
                FieldData data = new()
                {
                    ClassField = tf.Field,
                    Value = tf.GetData()
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
                foreach (IFillerBase tf in this.fillers)
                {
                    switch (tf.Field.Type)
                    {
                        case FieldType.Table:
                        case FieldType.Generator:
                            break;
                        default:
                            ISimpleFiller simple = (ISimpleFiller)tf;
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
            foreach (IFillerBase tf in this.ContentPanel.Children)
            {
                if (tf.Field.Id == id)
                {
                    tf.SetValue(data);
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
                if (this.OnSaveRequested?.Invoke(this) == false)
                {
                    return;
                }
                DocumentClassData docData = this.ClassData as DocumentClassData;
                string path = ProgramState.CurrentWorkspace.GetRuntimesTemplatesFolder();
                if (docData.Documents?.Count == 1)
                {
                    if (docData.Documents[1].File.EndsWith(".xlsx"))
                    {
                        DialogsManager.ShowExclamationDialog("Предпросмотр для Excel файлов недоступен.", "Рендеринг прерван");
                        return;
                    }
                    ProgramStatusBar.SetText("Рендеринг документа...");
                    DialogsManager.ShowWaitCursor(true);
                    string name = await this.GenerateDocument(docData.Documents[1], path, true);
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
            foreach (IFillerBase tf in this.ContentPanel.Children)
            {
                switch (tf.Field.Type)
                {
                    case FieldType.Generator:
                    case FieldType.Table:
                        break;
                    default:
                        output.Add(((ISimpleFiller)tf).GetValue());
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
