using Common;
using Incas.CreatedDocuments.Components;
using Incas.CreatedDocuments.Models;
using Incas.Templates.Components;
using Incas.Templates.Models;
using Incas.Templates.Views.Windows;
using IncasEngine.TemplateManager;
using Incubator_2.Common;
using Incubator_2.Forms.Templates;
using Incubator_2.Windows;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для UC_FileCreator.xaml
    /// </summary>
    public partial class UC_FileCreator : UserControl
    {
        private SGeneratedDocument document;
        private Template template;
        private List<Tag> tags;
        private List<UC_TagFiller> TagFillers = new();
        private List<TableFiller> Tables = new();
        private TemplateSettings templateSettings;

        public delegate void TagAction(int tag, string value);
        public event TagAction OnInsertRequested;
        public delegate void FileCreatorAction(UC_FileCreator creator);
        public event FileCreatorAction OnCreatorDestroy;
        public delegate void TagActionRecalculate(string tag);
        public event TagActionRecalculate OnRenameRequested;
        public bool SelectorChecked { get { return (bool)this.Selector.IsChecked; } }
        public UC_FileCreator(Template templ, ref TemplateSettings settings, List<Tag> tagsList)
        {
            this.InitializeComponent();
            this.tags = tagsList;
            this.template = templ;
            this.FillContentPanel();
            this.templateSettings = settings;
            this.NumberPrefix.Content = this.templateSettings.NumberPrefix;
            this.NumberPostfix.Content = this.templateSettings.NumberPostfix;
            if (this.template.type == TemplateType.Excel)
            {
                this.EyeButton.Visibility = Visibility.Collapsed;
                this.EyeButtonSeparator.Visibility = Visibility.Collapsed;
            }
            this.ExpanderButton.IsChecked = true;
        }
        public UC_FileCreator(List<Tag> tagsList) // dev mode
        {
            this.InitializeComponent();
            this.tags = tagsList;
            this.FillContentPanel();
            this.DocumentTools.Visibility = Visibility.Collapsed;
            this.DevModeLabel.Visibility = Visibility.Visible;
            this.RemoveButton.Visibility = Visibility.Collapsed;
            this.RemoveButtonRect.Visibility = Visibility.Collapsed;
            this.DefineNumberButton.IsEnabled = false;
            this.ExpanderButton.IsChecked = true;
        }

        private void FillContentPanel()
        {
            this.tags.ForEach(t =>
            {
                if (t.type != TagType.Table)
                {
                    UC_TagFiller tf = new(t);
                    tf.OnInsert += this.OnInsert;
                    tf.OnRename += this.OnRename;
                    tf.OnScriptRequested += this.OnScriptRequested;
                    this.ContentPanel.Children.Add(tf);
                    this.TagFillers.Add(tf);
                }
                else
                {
                    TableFiller tf = new(t);
                    this.ContentPanel.Children.Add(tf);
                    this.Tables.Add(tf);
                }
            });
        }

        private void OnScriptRequested(string script)
        {
            try
            {
                ScriptScope scope = ScriptManager.GetEngine().CreateScope();
                foreach (UC_TagFiller tf in this.TagFillers)
                {
                    scope.SetVariable(tf.tag.name.Replace(" ", "_"), tf.GetData());
                }
                ScriptManager.Execute(script, scope);
                foreach (UC_TagFiller tf in this.TagFillers)
                {
                    if (tf.tag.type != TagType.Generator)
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
                foreach (UC_TagFiller tf in this.TagFillers)
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
                foreach (UC_TagFiller tagfiller in this.TagFillers)
                {
                    if (tagfiller.tag.id == tag.tag)
                    {
                        tagfiller.SetValue(tag.value);
                        break;
                    }
                }
                foreach (TableFiller table in this.Tables)
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
            SGeneratedDocument result = new()
            {
                template = this.template.id,
                number = this.Number.Text,
                fullNumber = this.GetNumber(),
                status = this.document.status,
                fileName = this.Filename.Text
            };
            List<SGeneratedTag> filledTags = new();
            foreach (UC_TagFiller tf in this.TagFillers)
            {
                int id = tf.GetId();
                string name = tf.GetTagName();
                string value = tf.GetValue();
                if (tf.tag.type != TagType.LocalConstant)
                {
                    if (tf.tag.type is TagType.Generator or TagType.Date)
                    {
                        SGeneratedTag gtg = new()
                        {
                            tag = id,
                            value = tf.GetData()
                        };
                        filledTags.Add(gtg);
                    }
                    else
                    {
                        SGeneratedTag gt = new()
                        {
                            tag = id,
                            value = value
                        };
                        filledTags.Add(gt);
                    }
                }
            }
            foreach (TableFiller table in this.Tables)
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
                if (!string.IsNullOrEmpty(this.templateSettings.Validation))
                {
                    ScriptScope scope = ScriptManager.GetEngine().CreateScope();
                    scope.SetVariable("result", true);
                    scope.SetVariable("document_number", this.Number.Text);
                    scope.SetVariable("fields", new List<string>());
                    scope.SetVariable("failed_text", "Текст не установлен.");
                    foreach (UC_TagFiller tf in this.TagFillers)
                    {
                        scope.SetVariable(tf.tag.name.Replace(" ", "_"), tf.GetData());
                    }
                    ScriptManager.Execute(this.templateSettings.Validation, scope);
                    result = scope.GetVariable("result");
                    if (!result)
                    {
                        ProgramState.ShowExclamationDialog(scope.GetVariable("failed_text"));
                        dynamic fields = scope.GetVariable("fields");
                        foreach (UC_TagFiller tf in this.TagFillers)
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
                if (!string.IsNullOrEmpty(this.templateSettings.OnSaving))
                {
                    ScriptScope scope = ScriptManager.GetEngine().CreateScope();
                    foreach (UC_TagFiller tf in this.TagFillers)
                    {
                        scope.SetVariable(tf.tag.name.Replace(" ", "_"), tf.GetData());
                    }
                    scope.SetVariable("file_name", this.Filename.Text);
                    scope.SetVariable("document_number", this.Number.Text);
                    ScriptManager.Execute(this.templateSettings.OnSaving, scope);
                    this.Filename.Text = scope.GetVariable("file_name");
                    this.Number.Text = scope.GetVariable("document_number");
                    foreach (UC_TagFiller tf in this.TagFillers)
                    {
                        if (tf.tag.type != TagType.Generator)
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
            if (!string.IsNullOrWhiteSpace(this.templateSettings.FileNameTemplate))
            {
                string result = this.templateSettings.FileNameTemplate;
                foreach (UC_TagFiller tf in this.TagFillers)
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
                if (this.CustomValidate())
                {
                    this.ApplyNameByTemplate();
                    this.PlaySavingScript();
                    string newFile;
                    List<SGeneratedTag> filledTags = new();
                    switch (this.template.type)
                    {
                        case TemplateType.Word:
                            newFile = $"{newPath}\\{this.RemoveUnresolvedChars(this.Filename.Text)}.docx";
                            File.Copy(ProgramState.GetFullnameOfWordFile(this.template.path), newFile, true);
                            WordTemplator wt = new(newFile);
                            this.Dispatcher.Invoke(() =>
                            {
                                filledTags = wt.GenerateDocument(this.TagFillers, this.Tables, this.GetNumber(), async);
                            });
                            break;
                        case TemplateType.Excel:
                            newFile = $"{newPath}\\{this.RemoveUnresolvedChars(this.Filename.Text)}.xlsx";
                            File.Copy(ProgramState.GetFullnameOfExcelFile(this.template.path), newFile, true);
                            ExcelTemplator et = new(newFile);
                            this.Dispatcher.Invoke(() =>
                            {
                                filledTags = et.GenerateDocument(this.TagFillers, this.Tables, this.GetNumber(), async);
                            });
                            break;
                    }
                    if (save)
                    {
                        using GeneratedDocument doc = new();
                        doc.id = this.document.id;
                        doc.number = this.Number.Text;
                        doc.fullNumber = this.GetNumber();
                        doc.status = this.document.status;
                        doc.fileName = this.Filename.Text;
                        doc.template = this.template.id;
                        doc.templateName = category;
                        doc.SaveFilledTags(filledTags);
                        doc.AddRecord();
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
            foreach (UC_TagFiller tf in this.TagFillers)
            {
                if (tf.GetTagName() == tag)
                {
                    result = tf.GetValue();
                    break;
                }
            }
            this.Filename.Text = additive ? $"{prefix} {this.Filename.Text} {result} {postfix}".Trim() : $"{prefix} {result} {postfix}".Trim();

        }
        public List<string> GetExcelRow()
        {
            List<string> output = new();
            foreach (UC_TagFiller tf in this.TagFillers)
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
                System.IO.File.Copy(ProgramState.GetFullnameOfWordFile(this.template.path), newFile, true);
                WordTemplator wt = new(newFile);

                List<string> tagsToReplace = new();
                List<string> values = new();
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal,
                    new Action(() =>
                    {
                        foreach (UC_TagFiller tf in this.TagFillers)
                        {
                            string nameOf = tf.GetTagName();
                            string value = tf.GetValue();
                            tagsToReplace.Add(nameOf);
                            values.Add(value);
                        }
                        wt.Replace(tagsToReplace, values, false);
                        foreach (TableFiller tab in this.Tables)
                        {
                            wt.CreateTable(tab.tag.name, tab.DataTable);
                        }
                        string fileXPS = wt.TurnToXPS();
                        ProgramState.ShowWaitCursor(false);
                        PreviewWindow pr = new(fileXPS, !this.templateSettings.RequiresSave);
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
            this.CreateFile(ProgramState.TemplatesRuntime, "", false, false);
            string filename;
            switch (this.template.type)
            {
                case TemplateType.Word:
                default:
                    filename = $"{ProgramState.TemplatesRuntime}\\{this.RemoveUnresolvedChars(this.Filename.Text)}.docx";
                    break;
                case TemplateType.Excel:
                    filename = $"{ProgramState.TemplatesRuntime}\\{this.RemoveUnresolvedChars(this.Filename.Text)}.xlsx";
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
