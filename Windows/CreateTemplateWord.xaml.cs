using Common;
using IncasEngine.TemplateManager;
using Incubator_2.Forms;
using Incubator_2.ViewModels;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;


namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateTemplateWord.xaml
    /// </summary>
    public partial class CreateTemplateWord : Window
    {
        private Template template;
        private VM_Template vm;

        public delegate void Base();
        public event Base OnCreated;
        public CreateTemplateWord(Template te = null, string parents = null)
        {
            InitializeComponent();
            if (te == null)
            {
                this.template = new()
                {
                    parent = parents
                };
            }
            else
            {
                this.Title = $"Редактирование шаблона ({te.name})";
                this.template = te;
                this.GetTags();
            }
            this.vm = new VM_Template(this.template);
            this.DataContext = this.vm;
            ProgramState.ShowWaitCursor(false);
        }

        private void GetTags()
        {
            Tag tag = new();
            foreach (Tag t in tag.GetAllTagsByTemplate(this.template.id))
            {
                this.AddTag(t);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы", Justification = "<Ожидание>")]
        private void reviewClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new();
            fd.Filter = "Word и Excel|*.docx;*.xlsx";
            fd.InitialDirectory = ProgramState.TemplatesSourcesWordPath;
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string result;
                result = fd.SafeFileName;
                this.vm.Source = result;
                if (fd.FileName.EndsWith(".docx"))
                {
                    if (!fd.FileName.StartsWith(ProgramState.TemplatesSourcesWordPath))
                    {
                        File.Copy(fd.FileName, $"{ProgramState.TemplatesSourcesWordPath}\\{fd.SafeFileName}");
                    }
                    this.template.type = TemplateType.Word;
                }
                else if (fd.FileName.EndsWith(".xlsx"))
                {
                    if (!fd.FileName.StartsWith(ProgramState.TemplatesSourcesExcelPath))
                    {
                        File.Copy(fd.FileName, $"{ProgramState.TemplatesSourcesExcelPath}\\{fd.SafeFileName}");
                    }
                    this.template.type = TemplateType.Excel;
                }

            }
        }

        private bool CheckForSave()
        {
            if (!File.Exists(ProgramState.GetFullnameOfWordFile(this.template.path)) && !File.Exists(ProgramState.GetFullnameOfExcelFile(this.template.path)))
            {
                ProgramState.ShowErrorDialog($"Файл ({this.template.path}) не найден.", "Сохранение прервано");
                return false;
            }
            if (!this.template.path.EndsWith(".docx") && !this.template.path.EndsWith(".xlsx"))
            {
                ProgramState.ShowExclamationDialog($"Исходный файл шаблона должен быть с расширением .docx или .xlsx, любое другое расширение использовать нельзя.", "Сохранение прервано");
                return false;
            }
            if (string.IsNullOrWhiteSpace(this.nameOfTemplate.Text))
            {
                ProgramState.ShowExclamationDialog($"Неверное имя шаблона.", "Сохранение прервано");
                return false;
            }
            if (this.ContentPanel.Children.Count == 0 && string.IsNullOrWhiteSpace(this.template.parent))
            {
                ProgramState.ShowExclamationDialog($"Шаблон без единого тега не является шаблоном.", "Сохранение прервано");
                return false;
            }
            if (this.ContentPanel.Children.Count > 25)
            {
                ProgramState.ShowExclamationDialog($"Шаблон не может содержать более 25 тегов.\nРекомендуется использовать генераторы.", "Сохранение прервано");
                return false;
            }

            List<string> names = new();
            foreach (TagCreator tag in this.ContentPanel.Children)
            {
                if (!tag.Check())
                {
                    ProgramState.ShowExclamationDialog($"Тег \"{tag.tag.name}\" не заполнен.", "Сохранение прервано");
                    return false;
                }
                if (names.Contains(tag.TagName.Text))
                {
                    ProgramState.ShowExclamationDialog($"Найдено несколько тегов с именем [{tag.TagName.Text}].\nНазвания тегов должны быть уникальными.", "Сохранение прервано");
                    return false;
                }
                names.Add(tag.TagName.Text);
            }

            return true;
        }

        private void RemoveTagFromList(TagCreator tag)
        {
            this.ContentPanel.Children.Remove(tag);
        }

        private void saveClick(object sender, RoutedEventArgs e)
        {
            if (this.CheckForSave())
            {
                ProgramState.ShowWaitCursor();
                this.Close();
                this.template.SaveTemplateSettings(this.vm.GetSettings());
                if (this.template.id != 0)
                {
                    this.template.UpdateTemplate();
                    this.SaveTags(true);
                }
                else
                {
                    this.template.AddTemplate();
                    this.SaveTags(false);
                }
                OnCreated?.Invoke();
                ProgramState.ShowWaitCursor(false);
            }
        }

        private async void SaveTags(bool isEdit)
        {
            foreach (TagCreator tag in this.ContentPanel.Children)
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    tag.SaveTag(this.template.id, isEdit);
                });
            }
        }

        private void AddTag(Tag tag = null)
        {
            Tag t = new Tag();
            bool isNew = false;
            if (tag == null)
            {
                t.name = "Новый";
                isNew = true;
            }
            else
            {
                t = tag;
            }
            TagCreator tc = new(t, isNew);
            tc.onDelete += this.RemoveTagFromList;
            this.ContentPanel.Children.Add(tc);
        }

        private void AddTagClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.AddTag();
        }

        private void FindTagsInFile(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            bool CheckNameUniqueness(string name)
            {
                foreach (TagCreator creator in this.ContentPanel.Children)
                {
                    if (creator.tag.name == name)
                    {
                        return false;
                    }
                }
                return true;
            }
            string pathFile = ProgramState.GetFullnameOfWordFile(this.template.path);
            if (!File.Exists(pathFile))
            {
                ProgramState.ShowExclamationDialog($"Файл ({this.template.path}) не существует!\nТеги не могут быть обнаружены.", "Поиск невозможен");
                return;
            }
            try
            {
                WordTemplator wt = new(pathFile);
                foreach (string tagname in wt.FindAllTags())
                {
                    if (!CheckNameUniqueness(tagname))
                    {
                        continue;
                    }
                    Tag tag = new();
                    tag.name = tagname;
                    tag.visibleName = tagname;
                    this.AddTag(tag);
                }
            }
            catch (IOException)
            {
                ProgramState.ShowErrorDialog("Файл занят другим процесом. Его использование невозможно.");
            }
        }
        #region Design

        private void MinimizeAll(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (TagCreator tag in this.ContentPanel.Children)
            {
                tag.Minimize();
            }
        }

        private void MaximizeAll(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (TagCreator tag in this.ContentPanel.Children)
            {
                tag.Maximize();
            }
        }
        #endregion

        private void EditSourceClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string pathFile = "";
            switch (this.template.type)
            {
                case TemplateType.Excel:
                    pathFile = ProgramState.GetFullnameOfExcelFile(this.template.path);
                    break;
                case TemplateType.Word:
                    pathFile = ProgramState.GetFullnameOfWordFile(this.template.path);
                    break;
                default:
                    break;
            }

            if (!File.Exists(pathFile))
            {
                ProgramState.ShowExclamationDialog($"Файл ({this.template.path}) не существует!", "Действие прервано");
                return;
            }
            try
            {
                System.Diagnostics.Process proc = new();
                proc.StartInfo.FileName = pathFile;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog($"При попытке открытия файла возникла ошибка:\n{ex}");
            }
        }

        private void GetMoreInfoClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ProgramState.OpenWebPage("https://teletype.in/@incas/UM8Uf15j2fB");
        }

        private void AddCommandClick(object sender, RoutedEventArgs e)
        {
            CommandSettings cs = new();
            bool isSave = false;
            switch (((MenuItem)sender).Tag)
            {
                case "Save":
                    cs.Name = "Действия при сохранении";
                    cs.Script = this.vm.OnSavingScript;
                    isSave = true;
                    break;
                case "Validate":
                    cs.Name = "Валидация";
                    cs.Script = this.vm.ValidationScript;
                    break;
            }
            CreateTagCommand cc = new(cs);
            cc.ShowDialog();
            if (cc.Result == DialogStatus.Yes)
            {
                if (isSave)
                {
                    this.vm.OnSavingScript = cc.Command.Script;
                }
                else
                {
                    this.vm.ValidationScript = cc.Command.Script;
                }
            }
        }

        private void Sort(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            int order = 0;
            foreach (TagCreator tag in this.ContentPanel.Children)
            {
                order++;
                tag.SetOrderNumber(order);
            }
        }

        private void Download(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!this.CheckForSave())
            {
                return;
            }
            TemplatePort tp = new();
            FolderBrowserDialog folder = new FolderBrowserDialog();
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(this.template.parent))
                    {
                        DialogStatus ds = ProgramState.ShowQuestionDialog("Этот шаблон унаследован, вы можете применить все родительские теги к ниму.", "Учитывать родителей?", "Применить родителей", "Не применять");
                        if (ds == DialogStatus.Yes)
                        {
                            tp.FillData(this.template, true);
                        }
                        else
                        {
                            tp.FillData(this.template, false);
                        }
                    }
                    else
                    {
                        tp.FillData(this.template, false);
                    }
                    tp.ToFile(folder.SelectedPath);
                }
                catch (Exception ex)
                {
                    ProgramState.ShowErrorDialog("При попытке экспорта шаблона возникла ошибка:\n" + ex.Message);
                }
            }
        }

        private void Upload(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.template.id != 0)
            {
                ProgramState.ShowExclamationDialog("Невозможно применить импорт, поскольку это грозит утерей текущего шаблона", "Импорт прерван");
                return;
            }
            try
            {
                OpenFileDialog fd = new();
                fd.Filter = "Шаблоны INCAS|*.tinc";
                fd.InitialDirectory = ProgramState.TemplatesSourcesWordPath;
                if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    TemplatePort tp = JsonConvert.DeserializeObject<TemplatePort>(File.ReadAllText(fd.FileName));
                    
                    switch (tp.SourceTemplate.type)
                    {
                        case TemplateType.Word:
                            string wordpath = tp.SourceTemplate.path;
                            if (File.Exists(ProgramState.GetFullnameOfWordFile(tp.SourceTemplate.path)))
                            {
                                if (ProgramState.ShowQuestionDialog($"Исходный файл шаблона с именем \"{wordpath}\" уже существует в рабочем пространстве.\n" +
                                    $"Использовать его или переименовать предлагаемый файл из импортированного шаблона?", "Использовать старый шаблон?", "Использовать старый", "Переименовать предлагаемый") == DialogStatus.No)
                                {
                                    wordpath = ProgramState.ShowInputBox("Имя исходного файла", "Придумайте другое имя").Replace("\\", "").Replace(".docx", "") + ".docx";
                                    tp.SourceTemplate.path = wordpath;
                                }       
                            }
                            WordTemplator templator = new();
                            templator.DeserializeAndExtract(tp.Source, wordpath);
                            //File.WriteAllText(ProgramState.GetFullnameOfWordFile(wordpath), tp.Source, System.Text.Encoding.UTF8);
                            break;
                        case TemplateType.Excel:
                            string excelpath = tp.SourceTemplate.path;
                            if (File.Exists(ProgramState.GetFullnameOfExcelFile(tp.SourceTemplate.path)))
                            {
                                if (ProgramState.ShowQuestionDialog($"Исходный файл шаблона с именем \"{excelpath}\" уже существует в рабочем пространстве.\n" +
                                    $"Использовать его или переименовать предлагаемый файл из импортированного шаблона?", "Использовать старый шаблон?", "Использовать старый", "Переименовать предлагаемый") == DialogStatus.No)
                                {
                                    excelpath = ProgramState.ShowInputBox("Имя исходного файла", "Придумайте другое имя").Replace("\\", "").Replace(".xlsx", "") + ".xlsx";
                                    tp.SourceTemplate.path = excelpath;
                                }
                            }
                            File.WriteAllText(ProgramState.GetFullnameOfExcelFile(excelpath), tp.Source, System.Text.Encoding.UTF8);
                            break;
                    }
                    this.vm.ApplyNewTemplate(tp.SourceTemplate);
                    foreach (Tag t in tp.Tags)
                    {
                        this.AddTag(t);
                    }
                }
            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog("При попытке импорта шаблона возникла ошибка:\n" + ex.Message);
            }
        }

        private void DublicateNames(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (TagCreator tag in this.ContentPanel.Children)
            {
                if (string.IsNullOrWhiteSpace(tag.tag.visibleName))
                {
                    tag.DublicateName();
                }
            }
        }
    }
}
