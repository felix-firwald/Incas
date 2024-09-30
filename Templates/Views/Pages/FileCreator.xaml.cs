using Incas.Core.Classes;
using Incas.CreatedDocuments.Models;
using Incas.Templates.Components;
using Incas.Templates.Models;
using Incas.Templates.Views.Controls;
using Incas.Templates.Views.Windows;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Incas.Templates.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для FileCreator.xaml
    /// </summary>
    public partial class FileCreator : UserControl
    {
        private Template template;
        private List<Objects.Models.Field> fields;
        private List<TagFiller> TagFillers = [];
        private List<TableFiller> Tables = [];
        private TemplateSettings templateSettings;

        public delegate void TagAction(Guid tag, string value);
        public event TagAction OnInsertRequested;
        public delegate void FileCreatorAction(FileCreator creator);
        public event FileCreatorAction OnCreatorDestroy;
        public delegate void TagActionRecalculate(string tag);
        public event TagActionRecalculate OnRenameRequested;
        public bool SelectorChecked => (bool)this.Selector.IsChecked;
        public FileCreator(Template templ, ref TemplateSettings settings, List<Objects.Models.Field> tagsList)
        {
            this.InitializeComponent();
            this.fields = tagsList;
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
        public FileCreator(List<Objects.Models.Field> tagsList) // dev mode
        {
            this.InitializeComponent();
            this.fields = tagsList;
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
            this.fields.ForEach(t =>
            {
                if (t.Type != TagType.Table)
                {
                    TagFiller tf = new(t);
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
                foreach (TagFiller tf in this.TagFillers)
                {
                    scope.SetVariable(tf.field.Name.Replace(" ", "_"), tf.GetData());
                }
                ScriptManager.Execute(script, scope);
                foreach (TagFiller tf in this.TagFillers)
                {
                    if (tf.field.Type != TagType.Generator)
                    {
                        tf.SetValue(scope.GetVariable(tf.field.Name.Replace(" ", "_")));
                    }
                }
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog("При обработке скрипта на стороне формы возникла ошибка:\n" + ex.Message);
            }
        }

        private void OnInsert(Guid tag, string value)
        {
            OnInsertRequested?.Invoke(tag, value);
        }
        private void OnRename(string tag)
        {
            OnRenameRequested?.Invoke(tag);
        }
        public async void InsertTagValue(Guid tag, string value)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                foreach (TagFiller tf in this.TagFillers)
                {
                    if (tf.field.Id == tag)
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

        public void ApplyFromExcel(Dictionary<string, string> pairs)
        {
            foreach (KeyValuePair<string, string> pair in pairs)
            {
                foreach (TagFiller tf in this.ContentPanel.Children)
                {
                    if (tf.field.Name == pair.Key)
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
                fileName = this.Filename.Text
            };
            List<SGeneratedTag> filledTags = [];
            foreach (TagFiller tf in this.TagFillers)
            {
                Guid id = tf.GetId();
                string name = tf.GetTagName();
                string value = tf.GetData();
                SGeneratedTag gtg = new()
                {
                    tag = id,
                    value = tf.GetData()
                };
                filledTags.Add(gtg);
            }
            foreach (TableFiller table in this.Tables)
            {
                filledTags.Add(table.GetAsGeneratedTag());
            }
            return result;
        }
        private void PlaySavingScript()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.templateSettings.OnSaving))
                {
                    ScriptScope scope = ScriptManager.GetEngine().CreateScope();
                    foreach (TagFiller tf in this.TagFillers)
                    {
                        scope.SetVariable(tf.field.Name.Replace(" ", "_"), tf.GetData());
                    }
                    scope.SetVariable("file_name", this.Filename.Text);
                    scope.SetVariable("document_number", this.Number.Text);
                    ScriptManager.Execute(this.templateSettings.OnSaving, scope);
                    this.Filename.Text = scope.GetVariable("file_name");
                    this.Number.Text = scope.GetVariable("document_number");
                    foreach (TagFiller tf in this.TagFillers)
                    {
                        if (tf.field.Type != TagType.Generator)
                        {
                            tf.SetValue(scope.GetVariable(tf.field.Name.Replace(" ", "_")));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog("При выполнении скрипта возникла ошибка:\n" + ex.Message);
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
                foreach (TagFiller tf in this.TagFillers)
                {
                    result = result.Replace("[" + tf.GetTagName() + "]", tf.GetValue());
                }
                this.Filename.Text = result;
            }
        }
        public bool CreateFile(string newPath, bool async = true)
        {
            try
            {
                this.ApplyNameByTemplate();
                this.PlaySavingScript();
                string newFile;
                List<SGeneratedTag> filledTags = [];
                switch (this.template.type)
                {
                    case TemplateType.Word:
                        newFile = $"{newPath}\\{this.RemoveUnresolvedChars(this.Filename.Text)}.docx";
                        if (File.Exists(newFile))
                        {
                            File.Delete(newFile);
                        }
                        File.Copy(ProgramState.GetFullnameOfDocumentFile(this.template.path), newFile, true);
                        WordTemplator wt = new(newFile);
                        this.Dispatcher.Invoke(() =>
                        {
                            wt.GenerateDocument(this.TagFillers, this.Tables);
                        });
                        break;
                    case TemplateType.Excel:
                        newFile = $"{newPath}\\{this.RemoveUnresolvedChars(this.Filename.Text)}.xlsx";
                        if (File.Exists(newFile))
                        {
                            File.Delete(newFile);
                        }
                        File.Copy(ProgramState.GetFullnameOfDocumentFile(this.template.path), newFile, true);
                        ExcelTemplator et = new(newFile);
                        this.Dispatcher.Invoke(() =>
                        {
                            et.GenerateDocument(this.TagFillers, this.Tables);
                        });
                        break;
                }
                return true;
            }
            catch (GeneratorUndefinedStateException ex)
            {
                DialogsManager.ShowExclamationDialog(ex.Message, "Сохранение прервано");
                return false;
            }
            catch (IOException)
            {
                DialogsManager.ShowErrorDialog($"При доступе к файлу \"{this.Filename.Text}\" или его папке возникла ошибка.\n" +
                    $"Возможно существует файл с таким же именем, который уже открыт другим пользователем.\n" +
                    $"Файл будет пропущен.");
                return false;
            }
            catch (Exception e)
            {
                DialogsManager.ShowErrorDialog(e.Message);
                return false;
            }
        }

        public void RenameByTag(string tag, string prefix = "", string postfix = "", bool additive = false)
        {
            string result = "";
            foreach (TagFiller tf in this.TagFillers)
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
            List<string> output = [];
            foreach (TagFiller tf in this.TagFillers)
            {
                output.Add(tf.GetValue());
            }
            return output;
        }

        private async void PreviewCLick(object sender, MouseButtonEventArgs e)
        {
            if (this.template.type == TemplateType.Excel)
            {
                DialogsManager.ShowExclamationDialog("Предпросмотр недоступен для шаблонов Excel", "Действие недоступно");
                return;
            }
            DialogsManager.ShowWaitCursor();
            await System.Threading.Tasks.Task.Run(() =>
            {

                string newFile = $"{ProgramState.TemplatesRuntime}\\{DateTime.Now.ToString("yyMMddHHmmssff")}.docx";
                System.IO.File.Copy(ProgramState.GetFullnameOfDocumentFile(this.template.path), newFile, true);
                WordTemplator wt = new(newFile);

                List<string> tagsToReplace = [];
                List<string> values = [];
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal,
                    new Action(() =>
                    {
                        foreach (TagFiller tf in this.TagFillers)
                        {
                            string nameOf = tf.GetTagName();
                            string value = tf.GetValue();
                            tagsToReplace.Add(nameOf);
                            values.Add(value);
                        }
                        wt.Replace(tagsToReplace, values, false);
                        foreach (TableFiller tab in this.Tables)
                        {
                            wt.CreateTable(tab.field.Name, tab.DataTable);
                        }
                        string fileXPS = wt.TurnToXPS();
                        DialogsManager.ShowWaitCursor(false);
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
            DialogsManager.ShowWaitCursor();
            this.CreateFile(ProgramState.TemplatesRuntime, false);
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
                DialogsManager.ShowErrorDialog($"Не удалось открыть файл:\n{ex.Message}", "Действие невозможно");
            }
            DialogsManager.ShowWaitCursor(false);
        }
    }
}
