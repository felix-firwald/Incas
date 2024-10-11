using Incas.Core.Classes;
using Incas.Objects.Components;
using Incas.Objects.Exceptions;
using Incas.Objects.Views.Controls;
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

namespace Incas.Templates.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для FileCreator.xaml
    /// </summary>
    public partial class FileCreator : UserControl
    {
        private Template template;
        private List<Objects.Models.Field> fields;
        private List<FieldFiller> TagFillers = [];
        private List<FieldTableFiller> Tables = [];
        private TemplateSettings templateSettings;

        public delegate void TagAction(Guid tag, string value);
        public event TagAction OnInsertRequested;
        public delegate void FileCreatorAction(FileCreator creator);
        public event FileCreatorAction OnCreatorDestroy;
        public bool SelectorChecked => (bool)this.Selector.IsChecked;
        public FileCreator(Template templ, ref TemplateSettings settings, List<Objects.Models.Field> tagsList)
        {
            this.InitializeComponent();
            this.fields = tagsList;
            this.template = templ;
            this.FillContentPanel();
            this.templateSettings = settings;
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
            this.ExpanderButton.IsChecked = true;
        }

        private void FillContentPanel()
        {
            this.fields.ForEach(t =>
            {
                if (t.Type != FieldType.Table)
                {
                    FieldFiller tf = new(t);
                    tf.OnInsert += this.OnInsert;
                    tf.OnScriptRequested += this.OnScriptRequested;
                    this.ContentPanel.Children.Add(tf);
                    this.TagFillers.Add(tf);
                }
                else
                {
                    FieldTableFiller tf = new(t);
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
                foreach (FieldFiller tf in this.TagFillers)
                {
                    scope.SetVariable(tf.Field.Name.Replace(" ", "_"), tf.GetData());
                }
                ScriptManager.Execute(script, scope);
                foreach (FieldFiller tf in this.TagFillers)
                {
                    if (tf.Field.Type != FieldType.Generator)
                    {
                        tf.SetValue(scope.GetVariable(tf.Field.Name.Replace(" ", "_")));
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
        public async void InsertTagValue(Guid tag, string value)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                foreach (FieldFiller tf in this.TagFillers)
                {
                    if (tf.Field.Id == tag)
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
                foreach (FieldFiller tf in this.ContentPanel.Children)
                {
                    if (tf.Field.Name == pair.Key)
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
            foreach (FieldFiller tf in this.TagFillers)
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
            foreach (FieldTableFiller table in this.Tables)
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
                    foreach (FieldFiller tf in this.TagFillers)
                    {
                        scope.SetVariable(tf.Field.Name.Replace(" ", "_"), tf.GetData());
                    }
                    scope.SetVariable("file_name", this.Filename.Text);
                    ScriptManager.Execute(this.templateSettings.OnSaving, scope);
                    this.Filename.Text = scope.GetVariable("file_name");
                    foreach (FieldFiller tf in this.TagFillers)
                    {
                        if (tf.Field.Type != FieldType.Generator)
                        {
                            tf.SetValue(scope.GetVariable(tf.Field.Name.Replace(" ", "_")));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog("При выполнении скрипта возникла ошибка:\n" + ex.Message);
            }
        }
        private void ApplyNameByTemplate()
        {
            if (!string.IsNullOrWhiteSpace(this.templateSettings.FileNameTemplate))
            {
                string result = this.templateSettings.FileNameTemplate;
                foreach (FieldFiller tf in this.TagFillers)
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
                List<IFiller> fillers = [];
                foreach (IFiller filler in this.ContentPanel.Children)
                {
                    fillers.Add(filler);
                }
                switch (this.template.type)
                {
                    case TemplateType.Word:
                        newFile = $"{newPath}\\{this.RemoveUnresolvedChars(this.Filename.Text)}.docx";
                        WordTemplator wt = new(this.template.path, newFile);
                        if (async)
                        {
                            wt.GenerateDocumentAsync(fillers);
                        }
                        else
                        {
                            wt.GenerateDocument(fillers);
                        }                      
                        break;
                    case TemplateType.Excel:
                        newFile = $"{newPath}\\{this.RemoveUnresolvedChars(this.Filename.Text)}.xlsx";
                        ExcelTemplator et = new(this.template.path, newFile);
                        if (async)
                        {
                            et.GenerateDocumentAsync(fillers);
                        }
                        else
                        {
                            et.GenerateDocument(fillers);
                        }
                        break;
                }
                return true;
            }
            catch (NotNullFailed nn)
            {
                DialogsManager.ShowExclamationDialog(nn.Message, "Рендеринг прерван");
                return false;
            }
            catch (FieldDataFailed fd)
            {
                DialogsManager.ShowExclamationDialog(fd.Message, "Рендеринг прерван");
                return false;
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
            foreach (FieldFiller tf in this.TagFillers)
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
            foreach (FieldFiller tf in this.TagFillers)
            {
                output.Add(tf.GetValue());
            }
            return output;
        }

        private void PreviewCLick(object sender, MouseButtonEventArgs e)
        {
            if (this.template.type == TemplateType.Excel)
            {
                DialogsManager.ShowExclamationDialog("Предпросмотр недоступен для шаблонов Excel", "Действие недоступно");
                return;
            }
            try
            {
                DialogsManager.ShowWaitCursor();
                string newFile = $"{ProgramState.TemplatesRuntime}\\{DateTime.Now.ToString("yyMMddHHmmssff")}.docx";
                
                WordTemplator wt = new(this.template.path, newFile);

                List<IFiller> fillers = [];
                foreach (IFiller tf in this.ContentPanel.Children)
                {
                    fillers.Add(tf);
                }
                wt.GenerateDocument(fillers);
                string fileXPS = wt.TurnToXPS();
                DialogsManager.ShowWaitCursor(false);
                PreviewWindow pr = new(fileXPS, !this.templateSettings.RequiresSave);
                pr.Show();
            }
            catch (NotNullFailed nn)
            {
                DialogsManager.ShowExclamationDialog(nn.Message, "Рендеринг прерван");
            }
            catch (FieldDataFailed fd)
            {
                DialogsManager.ShowExclamationDialog(fd.Message, "Рендеринг прерван");
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }
        }

        private void OnSelectorChecked(object sender, RoutedEventArgs e)
        {

        }

        private void OnSelectorUnchecked(object sender, RoutedEventArgs e)
        {

        }

        private void OpenFileClick(object sender, MouseButtonEventArgs e)
        {
            try
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

                System.Diagnostics.Process proc = new();
                proc.StartInfo.FileName = filename;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
            catch (NotNullFailed nn)
            {
                DialogsManager.ShowExclamationDialog(nn.Message, "Рендеринг прерван");
            }
            catch (FieldDataFailed fd)
            {
                DialogsManager.ShowExclamationDialog(fd.Message, "Рендеринг прерван");
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog($"Не удалось открыть файл:\n{ex.Message}", "Действие невозможно");
            }
            DialogsManager.ShowWaitCursor(false);
        }
    }
}
