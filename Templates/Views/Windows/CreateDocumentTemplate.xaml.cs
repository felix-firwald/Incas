using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Incas.Objects.Exceptions;
using Incas.Templates.Components;
using Incas.Templates.Models;
using Incas.Templates.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Incas.Templates.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateTemplateWord.xaml
    /// </summary>
    public partial class CreateDocumentTemplate : Window
    {
        private TemplateViewModel vm;

        public delegate void Base();
        public event Base OnCreated;
        public CreateDocumentTemplate(Template te = null, string parents = null)
        {
            this.InitializeComponent();
            if (te == null)
            {
                te = new()
                {
                    parent = parents
                };
                this.vm = new TemplateViewModel(te);
            }
            else
            {
                this.Title = $"Редактирование шаблона ({te.name})";
                this.vm = new TemplateViewModel(te);
                this.GetTags();
            }

            this.DataContext = this.vm;
            DialogsManager.ShowWaitCursor(false);
        }

        private void GetTags()
        {
            Objects.Models.Field tag = new();
            foreach (Objects.Models.Field t in this.vm.Tags)
            {
                this.AddTag(t);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы", Justification = "<Ожидание>")]
        private void reviewClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new()
            {
                Filter = "Word и Excel|*.docx;*.xlsx",
                InitialDirectory = ProgramState.TemplatesSources
            };
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ApplySource(fd.FileName);
            }
        }

        private void ApplySource(string path)
        {
            if (path.EndsWith(".docx"))
            {
                this.vm.Type = TemplateType.Word;
            }
            else if (path.EndsWith(".xlsx"))
            {
                this.vm.Type = TemplateType.Excel;
            }
            string result = Path.GetFileName(path);
            this.vm.Source = result;
            if (!path.StartsWith(ProgramState.TemplatesSources)) // если файл еще не в каталоге рабочего пространства
            {
                if (File.Exists(ProgramState.GetFullnameOfDocumentFile(result))) // если в каталоге рабочего пространства есть файл с таким же именем
                {
                    if (DialogsManager.ShowQuestionDialog($"Файл с именем \"{result}\" уже существует в рабочем пространстве. Вы хотите выбрать присвоить выбранному файлу другое имя или использовать уже существующий файл?", "Файл уже существует", "Переименовать выбранный", "Использовать существующий") == DialogStatus.Yes)
                    {
                        string name = DialogsManager.ShowInputBox("Имя файла", "Введите имя файла без расширения").Replace(".xlsx", "").Replace(".docx", "");
                        switch (this.vm.Type)
                        {
                            case TemplateType.Excel:
                                name += ".xlsx";
                                break;
                            case TemplateType.Word:
                                name += ".docx";
                                break;
                        }
                        File.Copy(path, ProgramState.GetFullnameOfDocumentFile(name));
                    }
                }
                else
                {
                    File.Copy(path, ProgramState.GetFullnameOfDocumentFile(result));
                }
            }
        }

        private bool CheckForSave()
        {
            if (!File.Exists(ProgramState.GetFullnameOfDocumentFile(this.vm.Source)))
            {
                DialogsManager.ShowErrorDialog($"Файл ({this.vm.Source}) не найден в служебном каталоге.", "Сохранение прервано");
                return false;
            }
            if (!this.vm.Source.EndsWith(".docx") && !this.vm.Source.EndsWith(".xlsx"))
            {
                DialogsManager.ShowExclamationDialog("Исходный файл шаблона должен быть с расширением .docx или .xlsx, любое другое расширение использовать нельзя.", "Сохранение прервано");
                return false;
            }
            if (string.IsNullOrWhiteSpace(this.nameOfTemplate.Text))
            {
                DialogsManager.ShowExclamationDialog("Неверное имя шаблона.", "Сохранение прервано");
                return false;
            }
            if (string.IsNullOrEmpty(this.vm.Parents) && string.IsNullOrWhiteSpace(this.category.Text))
            {
                DialogsManager.ShowExclamationDialog("Категория не может быть пустой.", "Сохранение прервано");
                return false;
            }
            if (this.ContentPanel.Children.Count == 0 && string.IsNullOrWhiteSpace(this.vm.Parents))
            {
                DialogsManager.ShowExclamationDialog("Шаблон без единого тега не является шаблоном.", "Сохранение прервано");
                return false;
            }
            if (this.ContentPanel.Children.Count > 25)
            {
                DialogsManager.ShowExclamationDialog("Шаблон не может содержать более 25 тегов.\nРекомендуется использовать генераторы.", "Сохранение прервано");
                return false;
            }

            List<string> names = [];
            foreach (Objects.Views.Controls.FieldCreator tag in this.ContentPanel.Children)
            {
                if (names.Contains(tag.TagName.Text))
                {
                    DialogsManager.ShowExclamationDialog($"Найдено несколько тегов с именем [{tag.TagName.Text}].\nНазвания тегов должны быть уникальными.", "Сохранение прервано");
                    return false;
                }
                names.Add(tag.TagName.Text);
            }

            return true;
        }

        private bool RemoveTagFromList(Objects.Views.Controls.FieldCreator tag)
        {
            this.ContentPanel.Children.Remove(tag);
            return true;
        }

        private void saveClick(object sender, RoutedEventArgs e)
        {
            if (this.CheckForSave())
            {
                try
                {
                    DialogsManager.ShowWaitCursor();
                    this.SaveTags();
                    this.vm.SaveTemplate();
                    OnCreated?.Invoke();
                    this.Close();
                    DialogsManager.ShowWaitCursor(false);
                }
                catch (FieldDataFailed fd)
                {
                    DialogsManager.ShowExclamationDialog(fd.Message, "Сохранение прервано");
                }
            }
        }

        private void SaveTags()
        {
            List<Objects.Models.Field> tags = [];
            foreach (Objects.Views.Controls.FieldCreator tag in this.ContentPanel.Children)
            {
                tags.Add(tag.GetField());
            }
            this.vm.Tags = tags;
        }

        private void AddTag(Objects.Models.Field tag = null)
        {
            Objects.Models.Field t = new();
            if (tag == null)
            {
                t.Name = "Новый";
            }
            else
            {
                t = tag;
            }
            Objects.Views.Controls.FieldCreator fc = new(this.ContentPanel.Children.Count, t);
            fc.OnRemove += this.RemoveTagFromList;
            fc.OnMoveDownRequested += this.Fc_OnMoveDownRequested;
            fc.OnMoveUpRequested += this.Fc_OnMoveUpRequested;
            this.ContentPanel.Children.Add(fc);
        }

        private int Fc_OnMoveUpRequested(Objects.Views.Controls.FieldCreator t)
        {
            int position = this.ContentPanel.Children.IndexOf(t);
            if (position < this.ContentPanel.Children.Count - 1)
            {
                position += 1;
            }
            this.ContentPanel.Children.Remove(t);
            this.ContentPanel.Children.Insert(position, t);
            return position;
        }

        private int Fc_OnMoveDownRequested(Objects.Views.Controls.FieldCreator t)
        {
            int position = this.ContentPanel.Children.IndexOf(t);
            if (position > 0)
            {
                position -= 1;
            }
            this.ContentPanel.Children.Remove(t);
            this.ContentPanel.Children.Insert(position, t);
            return position;
        }

        private void AddTagClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.AddTag();
        }
        private void FindTagsInFileClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.FindTagsInFile();
        }

        private void FindTagsInFile()
        {
            bool CheckNameUniqueness(string name)
            {
                foreach (Objects.Views.Controls.FieldCreator creator in this.ContentPanel.Children)
                {
                    if (creator.vm.Source.Name == name)
                    {
                        return false;
                    }
                }
                return true;
            }
            string pathFile = ProgramState.GetFullnameOfDocumentFile(this.vm.Source);
            if (!File.Exists(pathFile))
            {
                DialogsManager.ShowExclamationDialog($"Файл ({this.vm.Source}) не существует!\nТеги не могут быть обнаружены.", "Поиск невозможен");
                return;
            }
            try
            {
                FileTemplator ft = new(pathFile);
                foreach (string tagname in ft.FindAllTags())
                {
                    if (!CheckNameUniqueness(tagname))
                    {
                        continue;
                    }
                    Objects.Models.Field tag = new()
                    {
                        Name = tagname,
                        VisibleName = tagname
                    };
                    this.AddTag(tag);
                }
            }
            catch (IOException)
            {
                DialogsManager.ShowErrorDialog("Файл занят другим процесом. Его использование невозможно.");
            }
        }
        #region Design

        private void MinimizeAll(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (Objects.Views.Controls.FieldCreator tag in this.ContentPanel.Children)
            {
                tag.Minimize();
            }
        }

        private void MaximizeAll(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (Objects.Views.Controls.FieldCreator tag in this.ContentPanel.Children)
            {
                tag.Maximize();
            }
        }
        #endregion

        private void EditSourceClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string pathFile = ProgramState.GetFullnameOfDocumentFile(this.vm.Source);

            if (!File.Exists(pathFile))
            {
                DialogsManager.ShowExclamationDialog($"Файл ({this.vm.Source}) не существует!", "Действие прервано");
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
                DialogsManager.ShowErrorDialog($"При попытке открытия файла возникла ошибка:\n{ex}");
            }
        }

        private void GetMoreInfoClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ProgramState.OpenWebPage("https://teletype.in/@incas/templates");
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
            foreach (Objects.Views.Controls.FieldCreator tag in this.ContentPanel.Children)
            {
                order++;
            }
        }

        private void Download(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!this.CheckForSave())
            {
                return;
            }
            TemplatePort tp = new();
            FolderBrowserDialog folder = new();
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(this.vm.Parents))
                    {
                        DialogStatus ds = DialogsManager.ShowQuestionDialog("Этот шаблон унаследован, вы можете применить все родительские теги к ниму.", "Учитывать родителей?", "Применить родителей", "Не применять");
                        if (ds == DialogStatus.Yes)
                        {
                            tp.FillData(this.vm.GetTemplate(), true);
                        }
                        else
                        {
                            tp.FillData(this.vm.GetTemplate(), false);
                        }
                    }
                    else
                    {
                        tp.FillData(this.vm.GetTemplate(), false);
                    }
                    tp.ToFile(folder.SelectedPath, ProgramState.GetFullnameOfDocumentFile(this.vm.Source), this.vm.Source);
                }
                catch (Exception ex)
                {
                    DialogsManager.ShowErrorDialog("При попытке экспорта шаблона возникла ошибка:\n" + ex.Message);
                }
            }
        }

        private void UploadClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.vm.Id != Guid.Empty)
            {
                DialogsManager.ShowExclamationDialog("Невозможно применить импорт, поскольку это грозит утерей текущего шаблона", "Импорт прерван");
                return;
            }
            OpenFileDialog fd = new()
            {
                Filter = "Шаблоны INCAS|*.tinc",
                InitialDirectory = ProgramState.TemplatesSources
            };
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.Upload(fd.FileName);
            }
        }

        private void Upload(string filename)
        {
            try
            {
                TemplatePort tp = new();
                tp.ParseData(filename);
                if (tp.Data != null)
                {
                    tp.GetSourceFile(filename, tp.Data.SourceTemplate.path);
                    this.vm.ApplyNewTemplate(tp.Data.SourceTemplate);
                    foreach (Objects.Models.Field t in tp.Data.Tags)
                    {
                        this.AddTag(t);
                    }
                }
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog("При попытке импорта шаблона возникла ошибка:\n" + ex.Message);
            }
        }

        private void DublicateNames(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (Objects.Views.Controls.FieldCreator tag in this.ContentPanel.Children)
            {
                if (string.IsNullOrWhiteSpace(tag.vm.Source.VisibleName))
                {
                    tag.DublicateName();
                }
            }
        }
        private List<Objects.Models.Field> GetTagsData()
        {
            List<Objects.Models.Field> tags = [];
            foreach (Objects.Views.Controls.FieldCreator tag in this.ContentPanel.Children)
            {
                tags.Add(tag.GetField());
            }
            return tags;
        }

        private void PreviewClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                UseTemplate ut = new(this.vm.NameOfTemplate, this.GetTagsData());
                ut.ShowDialog();
            }
            catch (FieldDataFailed fd)
            {
                DialogsManager.ShowExclamationDialog(fd.Message, "Предпросмотр прерван");
            }
        }

        private void Window_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                string result = Path.GetFileName(files[0]);
                if (files[0].EndsWith(".docx") || files[0].EndsWith(".xlsx"))
                {
                    this.ApplySource(files[0]);
                    this.FindTagsInFile();
                }
                else if (files[0].EndsWith(".tinc"))
                {
                    this.Upload(files[0]);
                }
            }
        }

        private void Window_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop)
                ? System.Windows.DragDropEffects.Copy
                : System.Windows.DragDropEffects.None;
        }
    }
}
