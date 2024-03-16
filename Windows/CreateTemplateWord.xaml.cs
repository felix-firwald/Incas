using Common;
using Incubator_2.Forms;
using Incubator_2.ViewModels;
using Incubator_2.Windows.Templates;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Forms;


namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateTemplateWord.xaml
    /// </summary>
    public partial class CreateTemplateWord : Window
    {
        Template template;
        private VM_Template VM_template;
        private readonly bool isEdit = false;

        public delegate void Base();
        public event Base OnCreated;
        public CreateTemplateWord(Template te = null)
        {
            InitializeComponent();
            if (te == null)
            {
                template = new Template();
            }
            else
            {
                isEdit = true;
                this.Title = $"Редактирование шаблона ({te.name})";
                template = te;
                this.FindInFileButton.IsEnabled = false;
                GetTags();
            }
            VM_template = new VM_Template(this.template);
            this.DataContext = VM_template;
            ProgramState.ShowWaitCursor(false);
        }

        private void GetTags()
        {
            Tag tag = new Tag();
            foreach(Tag t in tag.GetAllTagsByTemplate(template.id))
            {
                AddTag(t);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы", Justification = "<Ожидание>")]
        private void reviewClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "MS Word|*.docx";
            fd.InitialDirectory = ProgramState.TemplatesSourcesWordPath;
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string result;
                if (!fd.FileName.StartsWith(ProgramState.TemplatesSourcesWordPath))
                {
                    File.Copy(fd.FileName, $"{ProgramState.TemplatesSourcesWordPath}\\{fd.SafeFileName}");
                }
                result = fd.SafeFileName;
                this.VM_template.Source = result;
            }
        }

        private bool CheckForSave()
        {
            if (!File.Exists(ProgramState.GetFullnameOfWordFile(template.path)))
            {
                ProgramState.ShowErrorDialog($"Файл ({template.path}) не найден.","Сохранение прервано");
                return false;
            }
            if (!template.path.EndsWith(".docx"))
            {
                ProgramState.ShowExclamationDialog($"Исходный файл шаблона должен быть с расширением .docx, любое другое расширение использовать нельзя.", "Сохранение прервано");
                return false;
            }
            if (string.IsNullOrWhiteSpace(this.nameOfTemplate.Text))
            {
                ProgramState.ShowExclamationDialog($"Неверное имя шаблона.", "Сохранение прервано");
                return false;
            }
            if (this.ContentPanel.Children.Count == 0)
            {
                ProgramState.ShowExclamationDialog($"Шаблон без единого тега не является шаблоном.", "Сохранение прервано");
                return false;
            }

            List<string> names = new List<string>();
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
            if (CheckForSave())
            {
                ProgramState.ShowWaitCursor();
                this.Close();
                if (isEdit)
                {
                    template.UpdateTemplate();
                    SaveTags(true);
                }
                else
                {
                    template.AddTemplate(false);
                    SaveTags(false);
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
                    tag.SaveTag(template.id, isEdit);
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
            tc.onDelete += RemoveTagFromList;
            this.ContentPanel.Children.Add(tc);
        }

        private void AddTagClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AddTag();
        }

        private void FindTagsInFile(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string pathFile = ProgramState.GetFullnameOfWordFile(template.path);
            if (!File.Exists(pathFile))
            {
                ProgramState.ShowExclamationDialog($"Файл ({template.path}) не существует!\nТеги не могут быть обнаружены.", "Поиск невозможен");
                return;
            }
            try
            {
                this.ContentPanel.Children.Clear();
                WordTemplator wt = new WordTemplator(pathFile);
                foreach (string tagname in wt.FindAllTags())
                {
                    Tag tag = new Tag();
                    tag.name = tagname;
                    AddTag(tag);
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
            string pathFile = ProgramState.GetFullnameOfWordFile(template.path);
            if (!File.Exists(pathFile))
            {
                ProgramState.ShowExclamationDialog($"Файл ({template.path}) не существует!", "Действие прервано");
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

        private void AddTransferClick(object sender, RoutedEventArgs e)
        {
            List<Tag> tags = new();
            using (Tag tag = new Tag())
            {
                foreach (Tag t in tag.GetAllTagsByTemplate(template.id))
                {
                    tags.Add(t);
                }
            }

            CreateTransfer ct = new(tags);
            ct.ShowDialog();
        }
    }
}
