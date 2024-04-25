using Common;
using Incubator_2.Common;
using Incubator_2.Forms.Templates;
using Incubator_2.Models;
using Incubator_2.Windows;
using Microsoft.Scripting.Hosting;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Windows.ApplicationModel.Background;

namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для UC_FileCreator.xaml
    /// </summary>
    public partial class UC_FileCreator : UserControl
    {
        private SGeneratedDocument document;
        private bool IsCollapsed = false;
        private Template template;
        private List<Tag> tags;
        List<UC_TagFiller> TagFillers = new List<UC_TagFiller>();
        List<TableFiller> Tables = new List<TableFiller>();
        private TemplateSettings templateSettings;

        public delegate void TagAction(int tag, string value);
        public event TagAction OnInsertRequested;
        public delegate void FileCreatorAction(UC_FileCreator creator);
        public event FileCreatorAction OnCreatorDestroy;
        public delegate void TagActionRecalculate(string tag);
        public event TagActionRecalculate OnRenameRequested;
        public bool SelectorChecked { get { return (bool)this.Selector.IsChecked; } }
        public UC_FileCreator(Template templ, List<Tag> tagsList)
        {
            InitializeComponent();
            this.tags = tagsList;
            this.template = templ;
            FillContentPanel();
            templateSettings = templ.GetTemplateSettings();
            this.NumberPrefix.Content = templateSettings.NumberPrefix;
            this.NumberPostfix.Content = templateSettings.NumberPostfix;
            if (this.template.type == TemplateType.Excel)
            {
                this.EyeButton.Visibility = Visibility.Collapsed;
                this.EyeButtonSeparator.Visibility = Visibility.Collapsed;
            }
        }

        private void FillContentPanel()
        {
            this.tags.ForEach(t =>
            {
                if (t.type != TypeOfTag.Table)
                {
                    UC_TagFiller tf = new UC_TagFiller(t);
                    tf.OnInsert += OnInsert;
                    tf.OnRename += OnRename;
                    tf.OnScriptRequested += OnScriptRequested;
                    this.ContentPanel.Children.Add(tf);
                    TagFillers.Add(tf);
                }
                else
                {
                    TableFiller tf = new TableFiller(t);
                    this.ContentPanel.Children.Add(tf);
                    Tables.Add(tf);
                }
            });
        }

        private void OnScriptRequested(string script)
        {
            try
            {
                ScriptScope scope = ScriptManager.GetEngine().CreateScope();
                foreach (UC_TagFiller tf in TagFillers)
                {
                    scope.SetVariable(tf.tag.name.Replace(" ", "_"), tf.GetData());
                }
                ScriptManager.Execute(script, scope);
                foreach (UC_TagFiller tf in TagFillers)
                {
                    if (tf.tag.type != TypeOfTag.Generator)
                    {
                        tf.SetValue(scope.GetVariable(tf.tag.name.Replace(" ", "_")));
                    }
                }
            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog("При обработке скрипта на стороне формы возникла ошибка:\n" + ex.Message);
            }
        }

        private void OnInsert(int tag, string value)
        {
            OnInsertRequested?.Invoke(tag, value);
        }
        private void OnRename(string tag)
        {
            OnRenameRequested?.Invoke(tag);
        }
        public async void InsertTagValue(int tag, string value)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                foreach (UC_TagFiller tf in TagFillers)
                {
                    if (tf.tag.id == tag)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            tf.SetValue(value);
                        });

                        break;
                    }
                }
            });
        }

        public void ApplyRecord(SGeneratedDocument record)
        {
            if (record.filledTagsString == null)
            {
                ProgramState.ShowDatabaseErrorDialog("Не удалось получить информацию о полях документа.", "Запись повреждена");
                return;
            }
            this.document = record;
            this.Filename.Text = record.fileName;
            this.Number.Text = record.number;
            foreach (SGeneratedTag tag in record.GetFilledTags())
            {
                foreach (UC_TagFiller tagfiller in TagFillers)
                {
                    if (tagfiller.tag.id == tag.tag)
                    {
                        tagfiller.SetValue(tag.value);
                        break;
                    }
                }
                foreach (TableFiller table in Tables)
                {
                    if (table.tag.id == tag.tag)
                    {
                        table.SetData(tag.value);
                        break;
                    }
                }
            }
            switch (record.status)
            {
                case DocumentStatus.Approved:
                    this.Number.IsEnabled = false;
                    break;
                case DocumentStatus.Printed:
                    this.Number.IsEnabled = false;
                    break;
                case DocumentStatus.Done:
                    this.ContentPanel.IsEnabled = false;
                    this.Filename.IsEnabled = false;
                    this.Number.IsEnabled = false;
                    break;
            }
        }
        public void ApplyFromExcel(Dictionary<string, string> pairs)
        {
            foreach (KeyValuePair<string, string> pair in pairs)
            {
                foreach (UC_TagFiller tf in this.ContentPanel.Children)
                {
                    if (tf.tag.name == pair.Key)
                    {
                        tf.SetValue(pair.Value);
                        break;
                    }
                }
            }
        }
        public void Maximize()
        {
            this.MainBorder.Height = this.ContentPanel.Height + 40;
            this.IsCollapsed = false;
        }
        public void Minimize()
        {
            this.MainBorder.Height = 40;
            this.IsCollapsed = true;
        }

        private void ResizeClick(object sender, MouseButtonEventArgs e)
        {
            if (this.IsCollapsed)
            {
                Maximize();
            }
            else
            {
                Minimize();
            }

        }
        private void Remove(object sender, MouseButtonEventArgs e)
        {
            OnCreatorDestroy?.Invoke(this);

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
        public SGeneratedDocument GetGeneratedDocument()
        {
            SGeneratedDocument result = new();
            result.template = this.template.id;
            result.number = this.Number.Text;
            result.fullNumber = this.GetNumber();
            result.status = document.status;
            result.fileName = this.Filename.Text;
            List<SGeneratedTag> filledTags = new();
            foreach (UC_TagFiller tf in TagFillers)
            {
                int id = tf.GetId();
                string name = tf.GetTagName();
                string value = tf.GetValue();
                if (tf.tag.type != TypeOfTag.LocalConstant)
                {
                    if (tf.tag.type == TypeOfTag.Generator || tf.tag.type == TypeOfTag.Date)
                    {
                        SGeneratedTag gtg = new();
                        gtg.tag = id;
                        gtg.value = tf.GetData();
                        filledTags.Add(gtg);
                    }
                    else
                    {
                        SGeneratedTag gt = new();
                        gt.tag = id;
                        gt.value = value;
                        filledTags.Add(gt);
                    }
                }
            }
            foreach (TableFiller table in Tables)
            {
                filledTags.Add(table.GetAsGeneratedTag());
            }
            result.SaveFilledTags(filledTags);
            return result;
        }
        private bool CustomValidate()
        {
            try
            {
                if (this.document.status == DocumentStatus.Done)
                {
                    ProgramState.ShowAccessErrorDialog("Документ находится в статусе \"Завершен\", он будет пропущен.");
                    return false;
                }
                bool result = true;
                if (!string.IsNullOrEmpty(templateSettings.Validation))
                {
                    ScriptScope scope = ScriptManager.GetEngine().CreateScope();
                    scope.SetVariable("result", true);
                    scope.SetVariable("document_number", this.Number.Text);
                    scope.SetVariable("fields", new List<string>());
                    scope.SetVariable("failed_text", "Текст не установлен.");
                    foreach (UC_TagFiller tf in TagFillers)
                    {
                        scope.SetVariable(tf.tag.name.Replace(" ", "_"), tf.GetData());
                    }
                    ScriptManager.Execute(templateSettings.Validation, scope);
                    result = scope.GetVariable("result");
                    if (!result)
                    {
                        ProgramState.ShowExclamationDialog(scope.GetVariable("failed_text"));
                        dynamic fields = scope.GetVariable("fields");
                        foreach (UC_TagFiller tf in TagFillers)
                        {
                            if (fields.Contains(tf.tag.name.Replace(" ", "_")))
                            {
                                tf.MarkAsNotValidated();
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog("При выполнении скрипта валидации возникла ошибка:\n" + ex.Message);
                return true;
            }
        }
        private void PlaySavingScript()
        {
            try
            {
                if (!string.IsNullOrEmpty(templateSettings.OnSaving))
                {
                    ScriptScope scope = ScriptManager.GetEngine().CreateScope();
                    foreach (UC_TagFiller tf in TagFillers)
                    {
                        scope.SetVariable(tf.tag.name.Replace(" ", "_"), tf.GetData());
                    }
                    scope.SetVariable("file_name", this.Filename.Text);
                    scope.SetVariable("document_number", this.Number.Text);
                    ScriptManager.Execute(templateSettings.OnSaving, scope);
                    this.Filename.Text = scope.GetVariable("file_name");
                    this.Number.Text = scope.GetVariable("document_number");
                    foreach (UC_TagFiller tf in TagFillers)
                    {
                        if (tf.tag.type != TypeOfTag.Generator)
                        {
                            tf.SetValue(scope.GetVariable(tf.tag.name.Replace(" ", "_")));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog("При выполнении скрипта возникла ошибка:\n" + ex.Message);
            }
        }
        public string GetNumber()
        {
            return this.templateSettings.NumberPrefix + this.Number.Text + this.templateSettings.NumberPostfix;
        }
        private void ApplyNameByTemplate()
        {            
            if (!string.IsNullOrWhiteSpace(templateSettings.FileNameTemplate))
            {
                string result = templateSettings.FileNameTemplate;
                foreach (UC_TagFiller tf in TagFillers)
                {
                    result = result.Replace("[" + tf.GetTagName() + "]", tf.GetValue());
                }
                this.Filename.Text = result;
            }
        }
        public bool CreateFile(string newPath, string category, bool async = true, bool save = true)
        {
            try
            {
                if (CustomValidate())
                {
                    ApplyNameByTemplate();
                    PlaySavingScript();
                    string newFile;
                    List<SGeneratedTag> filledTags = new();
                    switch (this.template.type)
                    {
                        case TemplateType.Word:
                            newFile = $"{newPath}\\{RemoveUnresolvedChars(this.Filename.Text)}.docx";
                            File.Copy(ProgramState.GetFullnameOfWordFile(template.path), newFile, true);
                            WordTemplator wt = new WordTemplator(newFile);
                            this.Dispatcher.Invoke(() =>
                            {
                                filledTags = wt.GenerateDocument(TagFillers, Tables, GetNumber(), async);
                            });
                            break;
                        case TemplateType.Excel:
                            newFile = $"{newPath}\\{RemoveUnresolvedChars(this.Filename.Text)}.xlsx";
                            File.Copy(ProgramState.GetFullnameOfExcelFile(template.path), newFile, true);
                            ExcelTemplator et = new ExcelTemplator(newFile);                            
                            this.Dispatcher.Invoke(() =>
                            {
                                filledTags = et.GenerateDocument(TagFillers, Tables, GetNumber(), async);
                            });
                            break;
                    }
                    if (save)
                    {
                        using (GeneratedDocument doc = new())
                        {
                            doc.id = document.id;
                            doc.number = this.Number.Text;
                            doc.fullNumber = GetNumber();
                            doc.status = document.status;
                            doc.fileName = this.Filename.Text;
                            doc.template = this.template.id;
                            doc.templateName = category;
                            doc.SaveFilledTags(filledTags);
                            doc.AddRecord();
                        }
                    }
                    return true;
                }
                return false;
            }
            catch (GeneratorUndefinedStateException ex)
            {
                ProgramState.ShowExclamationDialog(ex.Message, "Сохранение прервано");
                return false;
            }
            catch (IOException)
            {
                ProgramState.ShowErrorDialog($"При доступе к файлу \"{this.Filename.Text}\" или его папке возникла ошибка.\n" +
                    $"Возможно существует файл с таким же именем, который уже открыт другим пользователем.\n" +
                    $"Файл будет пропущен.");
                return false;
            }
            catch (Exception e)
            {
                ProgramState.ShowErrorDialog(e.Message);
                return false;
            }
        }

        public void RenameByTag(string tag, string prefix = "", string postfix = "", bool additive = false)
        {
            string result = "";
            foreach (UC_TagFiller tf in TagFillers)
            {
                if (tf.GetTagName() == tag)
                {
                    result = tf.GetValue();
                    break;
                }
            }
            if (additive)
            {
                this.Filename.Text = $"{prefix} {this.Filename.Text} {result} {postfix}".Trim();
            }
            else
            {
                this.Filename.Text = $"{prefix} {result} {postfix}".Trim();
            }

        }
        public List<string> GetExcelRow()
        {
            List<string> output = new();
            foreach (UC_TagFiller tf in TagFillers)
            {
                output.Add(tf.GetValue());
            }
            return output;
        }

        private async void PreviewCLick(object sender, MouseButtonEventArgs e)
        {
            if (this.template.type == TemplateType.Excel)
            {
                ProgramState.ShowExclamationDialog("Предпросмотр недоступен для шаблонов Excel", "Действие недоступно");
                return;
            }
            ProgramState.ShowWaitCursor();
            await System.Threading.Tasks.Task.Run(() =>
            {

                string newFile = $"{ProgramState.TemplatesRuntime}\\{DateTime.Now.ToString("yyMMddHHmmssff")}.docx";
                System.IO.File.Copy(ProgramState.GetFullnameOfWordFile(template.path), newFile, true);
                WordTemplator wt = new WordTemplator(newFile);

                List<string> tagsToReplace = new List<string>();
                List<string> values = new List<string>();
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal,
                    new Action(() =>
                    {
                        foreach (UC_TagFiller tf in TagFillers)
                        {
                            string nameOf = tf.GetTagName();
                            string value = tf.GetValue();
                            tagsToReplace.Add(nameOf);
                            values.Add(value);
                        }
                        wt.Replace(tagsToReplace, values, false);
                        foreach (TableFiller tab in Tables)
                        {
                            wt.CreateTable(tab.tag.name, tab.DataTable);
                        }
                        string fileXPS = wt.TurnToXPS();
                        ProgramState.ShowWaitCursor(false);
                        PreviewWindow pr = new PreviewWindow(fileXPS, !this.templateSettings.RequiresSave);
                        pr.Show();
                    })
                );
            });

        }

        private void OnSelectorChecked(object sender, RoutedEventArgs e)
        {

        }

        private void OnSelectorUnchecked(object sender, RoutedEventArgs e)
        {

        }

        private void OpenFileClick(object sender, MouseButtonEventArgs e)
        {
            if (this.document.status == DocumentStatus.Done)
            {
                ProgramState.ShowAccessErrorDialog("Функция генерации документа недоступна, пока он находится в статусе \"Завершен\".");
                return;
            }
            if (this.templateSettings.RequiresSave)
            {
                ProgramState.ShowExclamationDialog("Этот тип документа требует сохранения в историю, для создания файла используйте кнопку \"Создать файлы по шаблону\".", "Генерация прервана");
                return;
            }
            ProgramState.ShowWaitCursor();
            CreateFile(ProgramState.TemplatesRuntime, "", false, false);
            string filename;
            switch (this.template.type)
            {
                case TemplateType.Word:
                default:
                    filename = $"{ProgramState.TemplatesRuntime}\\{RemoveUnresolvedChars(this.Filename.Text)}.docx";
                    break;
                case TemplateType.Excel:
                    filename = $"{ProgramState.TemplatesRuntime}\\{RemoveUnresolvedChars(this.Filename.Text)}.xlsx";
                    break;
            }            
            try
            {
                System.Diagnostics.Process proc = new();
                proc.StartInfo.FileName = filename;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog($"Не удалось открыть файл:\n{ex.Message}", "Действие невозможно");
            }
            ProgramState.ShowWaitCursor(false);
        }
    }
}
