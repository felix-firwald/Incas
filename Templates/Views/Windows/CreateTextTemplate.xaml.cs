using Incas.Common;
using Incas.Core.Views.Windows;
using Incas.Templates.Components;
using Incas.Templates.Models;
using Incas.Templates.ViewModels;
using IncasEngine.TemplateManager;
using Incubator_2.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incas.Templates.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateTextTemplate.xaml
    /// </summary>
    public partial class CreateTextTemplate : Window
    {
        public Template template;
        private TemplateViewModel vm;

        public delegate void Base();
        public event Base OnCreated;
        public CreateTextTemplate() // new
        {
            this.InitializeComponent();
            this.template = new()
            {
                type = TemplateType.Text
            };
            this.vm = new TemplateViewModel(this.template);
            this.DataContext = this.vm;
        }
        public CreateTextTemplate(Template te) // edit
        {
            this.InitializeComponent();
            this.Title = $"Редактирование генератора ({te.name})";
            this.template = te;
            this.vm = new TemplateViewModel(this.template);
            this.DataContext = this.vm;
            this.GetTags();

        }
        private void GetTags()
        {
            Tag tag = new();
            foreach (Tag t in tag.GetAllTagsByTemplate(this.template.id))
            {
                this.AddTag(t);
            }
        }
        private async void SaveTags()
        {
            foreach (TagCreator tag in this.ContentPanel.Children)
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    tag.SaveTag(this.template.id);
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
        private void RemoveTagFromList(TagCreator tag)
        {
            this.ContentPanel.Children.Remove(tag);
        }

        private void AddTagClick(object sender, MouseButtonEventArgs e)
        {
            this.AddTag();
        }
        private void GetTagsFromTextClick(object sender, MouseButtonEventArgs e)
        {
            List<string> result = [];
            Regex regex = new Regex(@"\[[A-Za-zА-Яа-я ]*\]"); // @"\[(\w*)\]"   @"\[(\.*)\]"
            MatchCollection matches = regex.Matches(this.source.Text);

            foreach (Match match in matches)
            {
                result.Add(match.Value.TrimStart('[').TrimEnd(']'));
            }
            foreach (string tagname in result)
            {
                Tag tag = new Tag
                {
                    name = tagname
                };
                this.AddTag(tag);
            }
        }
        private void MinimizeAllClick(object sender, MouseButtonEventArgs e)
        {
            foreach (TagCreator tag in this.ContentPanel.Children)
            {
                tag.Minimize();
            }
        }
        private void MaximizeAllClick(object sender, MouseButtonEventArgs e)
        {
            foreach (TagCreator tag in this.ContentPanel.Children)
            {
                tag.Maximize();
            }
        }
        private bool CheckForSave()
        {
            if (string.IsNullOrWhiteSpace(this.nameOfTemplate.Text))
            {
                ProgramState.ShowExclamationDialog($"Неверное имя генератора.", "Сохранение прервано");
                return false;
            }
            if (this.ContentPanel.Children.Count == 0)
            {
                ProgramState.ShowExclamationDialog($"Генератор без единого тега не является генератором.", "Сохранение прервано");
                return false;
            }

            List<string> names = [];
            foreach (TagCreator tag in this.ContentPanel.Children)
            {
                if (names.Contains(tag.TagName.Text))
                {
                    ProgramState.ShowExclamationDialog($"Найдено несколько тегов с именем [{tag.TagName.Text}].\nНазвания тегов должны быть уникальными.", "Сохранение прервано");
                    return false;
                }
                if (tag.tag.type == TagType.Table)
                {
                    ProgramState.ShowExclamationDialog($"В генераторах нельзя использовать таблицы!", "Сохранение прервано");
                    return false;
                }
                names.Add(tag.TagName.Text);
            }


            return true;
        }
        private void saveClick(object sender, RoutedEventArgs e)
        {
            if (this.CheckForSave())
            {
                ProgramState.ShowWaitCursor();
                this.Close();
                this.vm.SaveTemplate();
                this.SaveTags();
                OnCreated?.Invoke();
                ProgramState.ShowWaitCursor(false);
            }
        }

        private void AddCommandClick(object sender, RoutedEventArgs e)
        {
            CommandSettings cs = new();
            string tagData = ((MenuItem)sender).Tag.ToString();
            switch (tagData)
            {
                case "Open":
                    cs.Name = "Действия при открытии";
                    cs.Script = this.vm.OnOpeningScript;
                    break;
                case "Save":
                    cs.Name = "Действия при сохранении";
                    cs.Script = this.vm.OnSavingScript;
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
                switch (tagData)
                {
                    case "Open":
                        this.vm.OnOpeningScript = cc.Command.Script;
                        break;
                    case "Save":
                        this.vm.OnSavingScript = cc.Command.Script;
                        break;
                    case "Validate":
                        this.vm.ValidationScript = cc.Command.Script;
                        break;
                }
            }
        }
    }
}
