using Common;
using Incubator_2.Common;
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
using Telegram.Bot.Types;

namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для UC_FileCreator.xaml
    /// </summary>
    public partial class UC_FileCreator : UserControl
    {
        private bool IsCollapsed = false;
        private Template template;
        private List<Tag> tags;
        List<UC_TagFiller> TagFillers = new List<UC_TagFiller>();
        List<TableFiller> Tables = new List<TableFiller>();

        public delegate void TagAction(int tag, string value);
        public event TagAction OnInsertRequested;
        public delegate void TagActionRecalculate(string tag);
        public event TagActionRecalculate OnRenameRequested;
        public bool SelectorChecked { get { return (bool)this.Selector.IsChecked; } }
        public UC_FileCreator(Template templ, List<Tag> tagsList)
        {
            InitializeComponent();
            this.tags = tagsList;
            this.template = templ;
            FillContentPanel();
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
                    tf.SetValue(scope.GetVariable(tf.tag.name.Replace(" ", "_")));
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
        
        public void ApplyRecord(string fileName, List<SGeneratedTag> record)
        {
            if (record == null)
            {
                ProgramState.ShowDatabaseErrorDialog("Не удалось получить информацию о полях документа.", "Запись повреждена");
                return;
            }
            this.Filename.Text = fileName;
            foreach (SGeneratedTag tag in record)
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
            Panel parentPanel = (Panel)this.Parent;
            parentPanel.Children.Remove(this);
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
            result.fileName = this.Filename.Text;
            List<SGeneratedTag> filledTags = new();
            foreach (UC_TagFiller tf in TagFillers)
            {
                int id = tf.GetId();
                string name = tf.GetTagName();
                string value = tf.GetValue();
                if (tf.tag.type != TypeOfTag.LocalConstant)
                {
                    SGeneratedTag gt = new();
                    gt.tag = id;
                    gt.value = value;
                    filledTags.Add(gt);
                }
            }
            foreach (TableFiller table in Tables)
            {               
                filledTags.Add(table.GetAsGeneratedTag());
            }
            result.SaveFilledTags(filledTags);
            return result;
        }
        public void CreateFile(string newPath, bool async = true, bool save = true)
        {
            try
            {
                string newFile = $"{newPath}\\{RemoveUnresolvedChars(this.Filename.Text)}.docx";
                System.IO.File.Copy(ProgramState.GetFullnameOfWordFile(template.path), newFile, true);
                List<SGeneratedTag> filledTags = new();
                WordTemplator wt = new WordTemplator(newFile);

                List<string> tagsToReplace = new List<string>();
                List<string> values = new List<string>();
                this.Dispatcher.Invoke(() =>
                {
                    foreach (TableFiller tab in Tables)
                    {
                        wt.CreateTable(tab.tag.name, tab.DataTable);
                        filledTags.Add(tab.GetAsGeneratedTag());
                    }
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
                        tagsToReplace.Add(name);
                        values.Add(value);
                    }
                    wt.Replace(tagsToReplace, values, async);
                });
                if (save)
                {
                    using (GeneratedDocument doc = new())
                    {
                        doc.fileName = this.Filename.Text;
                        doc.template = this.template.id;
                        doc.templateName = this.template.name;
                        doc.SaveFilledTags(filledTags);
                        doc.AddRecord();
                    }
                }
            }
            catch (IOException)
            {
                ProgramState.ShowErrorDialog($"При доступе к файлу \"{this.Filename.Text}\" или его папке возникла ошибка.\n" +
                    $"Возможно существует файл с таким же именем, который уже открыт другим пользователем.\n" +
                    $"Файл будет пропущен.");
            }
            catch (Exception e)
            {
                ProgramState.ShowErrorDialog(e.Message);
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
                        PreviewWindow pr = new PreviewWindow(fileXPS);
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
            ProgramState.ShowWaitCursor();
            CreateFile(ProgramState.TemplatesRuntime, false, false);
            string filename = $"{ProgramState.TemplatesRuntime}\\{RemoveUnresolvedChars(this.Filename.Text)}.docx";
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
