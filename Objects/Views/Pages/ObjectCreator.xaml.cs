using Incas.Core.Classes;
using Incas.Core.Interfaces;
using Incas.Objects.AutoUI;
using Incas.Objects.Components;
using Incas.Objects.Interfaces;
using Incas.Objects.Models;
using Incas.Objects.Views.Controls;
using Incas.Objects.Views.Windows;
using Incas.Templates.Components;
using Incas.Templates.Views.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Incas.Objects.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ObjectCreator.xaml
    /// </summary>
    public partial class ObjectCreator : System.Windows.Controls.UserControl, ICollapsible
    {
        public Components.Object Object { get; set; }
        public Class Class { get; set; }
        public ClassData ClassData { get; set; }
        public delegate bool ObjectCreatorData(ObjectCreator creator);
        public delegate void FieldCopyAction(Guid id, string text);
        public event FieldCopyAction OnInsertRequested;
        public event ObjectCreatorData OnUpdated;
        public event ObjectCreatorData OnSaveRequested;
        public event ObjectCreatorData OnRemoveRequested;
        private bool Locked = false;
        public ObjectCreator(Class source, ClassData data, Components.Object obj = null)
        {
            this.InitializeComponent();
            this.Class = source;
            this.ClassData = data;
            this.FillContentPanel();
            if (obj != null)
            {
                this.ApplyObject(obj);
            }
            else
            {
                this.Object = new();
            }
            if (data.ClassType != ClassType.Document)
            {
                this.RenderArea.Visibility = Visibility.Collapsed;
                if (data.ClassType == ClassType.Generator)
                {
                    this.SaveArea.Visibility = Visibility.Collapsed;
                }
            }           
            else
            {
                this.ApplyTerminated();
            }
            this.ExpanderButton.IsChecked = true;
            this.ApplyAuthorConstraint();
        }
        public ObjectCreator(ClassData data) // dev mode
        {
            this.InitializeComponent();
            this.ClassData = data;
            this.FillContentPanel();
            this.Object = new();
            this.DocumentTools.Visibility = Visibility.Collapsed;
            this.RemoveButton.Visibility = Visibility.Collapsed;
        }
        private void ApplyTerminated()
        {
            if (this.Object.Terminated == true)
            {
                this.RenderArea.Visibility = Visibility.Collapsed;
                this.SaveArea.Visibility = Visibility.Collapsed;
                this.TerminatedIcon.Visibility = Visibility.Visible;
                this.ContentPanel.IsEnabled = false;
                SolidColorBrush color = new(System.Windows.Media.Color.FromRgb(52, 201, 36));
                this.Separator.Fill = color;
                //System.Windows.Controls.Label label = new()
                //{
                //    Content = "Процесс завершен.",
                //    Margin = new Thickness(5),
                //    Foreground = color
                //};
                //this.ContentPanel.Children.Insert(0, label);
            }
        }
        private void ApplyAuthorConstraint()
        {
            if (this.Object.Id != Guid.Empty && this.ClassData.EditByAuthorOnly == true && this.Object.AuthorId != ProgramState.CurrentUser.id)
            {
                this.Locked = true;
                this.ContentPanel.IsEnabled = false;
                System.Windows.Controls.Label label = new()
                {
                    Content = "Вы не можете редактировать этот объект, поскольку не являетесь его автором.",
                    Margin = new Thickness(5),
                    Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0))
                };
                this.ContentPanel.Children.Insert(0, label);
            }
        }
        private void FillContentPanel()
        {
            foreach (Objects.Models.Field f in this.ClassData.Fields)
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
                        ff.OnDatabaseObjectCopyRequested += this.Tf_OnDatabaseObjectCopyRequested;
                        this.ContentPanel.Children.Add(ff);
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
                        break;
                }
            }
        }

        private void Tf_OnDatabaseObjectCopyRequested(IFillerBase sender)
        {
            BindingData bd = new()
            {
                Class = this.Class.identifier,
                Field = sender.Field.Id
            };
            DatabaseSelection ds = new(bd);
            ds.ShowDialog();
            foreach (Components.FieldData field in ds.SelectedObject?.Fields)
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
        public void ApplyObject(Components.Object obj)
        {
            this.Object = obj;
            this.ObjectName.Text = obj.Name;
            foreach (Components.FieldData data in obj.Fields)
            {
                foreach (IFillerBase filler in this.ContentPanel.Children)
                {
                    if (filler.Field.Id == data.ClassField.Id)
                    {                     
                        filler.SetValue(data.Value);         
                        break;
                    }
                }
            }
            foreach (IFillerBase filler in this.ContentPanel.Children)
            {
                if (filler.Field.Type == FieldType.Generator)
                {
                    ((IGeneratorFiller)filler).ApplyObjectsBy(this.Class, obj.Id);
                }
            }
        }
        public Components.Object PullObject()
        {
            if (this.Locked == true)
            {
                throw new Exceptions.AuthorFailed($"Объект с именем \"{this.Object.Name}\" не может быть модифицирован, поскольку не вы являетесь его автором.");
            }
            this.UpdateName();
            this.Object.Name = this.ObjectName.Text;
            if (this.Object.Fields == null)
            {
                this.Object.Fields = [];
            }
            this.Object.Fields.Clear();
            foreach (IFillerBase tf in this.ContentPanel.Children)
            {
                Components.FieldData data = new()
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
            if (this.ClassData.InsertTemplateName)
            {
                templatePart = " (" + template + ")";
            }
            result = $"{folder}\\{this.RemoveUnresolvedChars(this.ObjectName.Text)}{templatePart}.{extension}";
            return result;
        }
        public void GenerateDocument()
        {
            string name = this.ObjectName.Text;
            string path = "";
            if (this.ClassData.Templates?.Count == 1)
            {
                if (DialogsManager.ShowFolderBrowserDialog(ref path) == true)
                {
                    this.GenerateDocument(this.ClassData.Templates[1], path);
                }
            }
            else if (this.ClassData.Templates?.Count > 1)
            {
                TemplateSelection ts = new(this.ClassData);
                if (ts.ShowDialog("Выбор шаблона", Icon.Magic) == true)
                {
                    if (DialogsManager.ShowFolderBrowserDialog(ref path) == true)
                    {
                        this.GenerateDocument(ts.GetSelectedPath(), path);
                    }
                }
            }
            else
            {

            }
        }
        public string GenerateDocument(TemplateData templateData, string folder, bool async = true)
        {
            string newFile = "";
            string oldFile = templateData.File;
            ITemplator templ = null;
            List<IFillerBase> fillers = new();
            foreach (IFillerBase filler in this.ContentPanel.Children)
            {
                fillers.Add(filler);
            }
            try
            {
                if (oldFile.EndsWith(".docx"))
                {
                    newFile = this.GetNameOfFile(folder, templateData.Name, "docx");
                    WordTemplator wt = new(oldFile, newFile);
                    templ = wt;                
                }
                else if (oldFile.EndsWith(".xlsx"))
                {
                    newFile = this.GetNameOfFile(folder, templateData.Name, "xlsx");
                    ExcelTemplator et = new(oldFile, newFile);
                    templ = et;                  
                }
                if (async)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        templ.GenerateDocumentAsync(fillers);
                    });               
                }
                else
                {
                    templ.GenerateDocument(fillers);
                }
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
            }
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrEmpty(this.ObjectName.Text))
            {
                name = "Объект от " + DateTime.Now;
                this.ObjectName.Text = name;
                return name;
            }
            foreach (IFillerBase tf in this.ContentPanel.Children)
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

        private void SaveClick(object sender, MouseButtonEventArgs e)
        {
            this.OnSaveRequested?.Invoke(this);
        }

        private void PreviewCLick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.OnSaveRequested?.Invoke(this) == false)
                {
                    return;
                }
                string path = ProgramState.TemplatesRuntime;
                if (this.ClassData.Templates?.Count == 1)
                {
                    if (this.ClassData.Templates[1].File.EndsWith(".xlsx"))
                    {
                        DialogsManager.ShowExclamationDialog("Предпросмотр для Excel файлов недоступен.", "Рендеринг прерван");
                        return;
                    }
                    DialogsManager.ShowWaitCursor(true);
                    string name = this.GenerateDocument(this.ClassData.Templates[1], path, false);
                    WordTemplator wt = new(name);
                    string fileXPS = wt.TurnToXPS();
                    DialogsManager.ShowWaitCursor(false);
                    PreviewWindow pr = new(fileXPS, true);
                    pr.Show();
                }
                else if (this.ClassData.Templates?.Count > 1)
                {
                    TemplateSelection ts = new(this.ClassData);
                    if (ts.ShowDialog("Выбор шаблона", Icon.Magic) == true)
                    {
                        if (ts.GetSelectedPath().File.EndsWith(".xlsx"))
                        {
                            DialogsManager.ShowExclamationDialog("Предпросмотр для Excel файлов недоступен.", "Рендеринг прерван");
                            return;
                        }
                        string name = this.GenerateDocument(ts.GetSelectedPath(), path, false);
                        WordTemplator wt = new(name);
                        string fileXPS = wt.TurnToXPS();
                        DialogsManager.ShowWaitCursor(false);
                        PreviewWindow pr = new(fileXPS, true);
                        pr.Show();
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

        private void RemoveClick(object sender, MouseButtonEventArgs e)
        {
            this.OnRemoveRequested?.Invoke(this);
        }
    }
}
