using Common;
using Incubator_2.Forms;
using Incubator_2.ViewModels;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Words.NET;

namespace Incubator_2.Windows.Templates
{
    /// <summary>
    /// Логика взаимодействия для CreateTextTemplate.xaml
    /// </summary>
    public partial class CreateTextTemplate : Window
    {
        public Template template;
        private VM_Template VM_template;
        private readonly bool isEdit = false;

        public delegate void Base();
        public event Base OnCreated;
        public CreateTextTemplate() // new
        {
            InitializeComponent();
            template = new Template();
            template.type = TemplateType.Text;
            VM_template = new VM_Template(this.template);
            this.DataContext = VM_template;
        }
        public CreateTextTemplate(Template te) // edit
        {
            InitializeComponent();
            isEdit = true;
            this.Title = $"Редактирование генератора ({te.name})";
            template = te;
            VM_template = new VM_Template(this.template);
            this.DataContext = VM_template;
            GetTags();
            
        }
        private void GetTags()
        {
            Tag tag = new Tag();
            foreach (Tag t in tag.GetAllTagsByTemplate(template.id))
            {
                AddTag(t);
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
        private void RemoveTagFromList(TagCreator tag)
        {
            this.ContentPanel.Children.Remove(tag);
        }

        private void AddTagClick(object sender, MouseButtonEventArgs e)
        {
            AddTag();
        }
        private void GetTagsFromTextClick(object sender, MouseButtonEventArgs e)
        {
            List<string> result = new List<string>();
            Regex regex = new Regex(@"\[[A-Za-zА-Яа-я ]*\]"); // @"\[(\w*)\]"   @"\[(\.*)\]"
            MatchCollection matches = regex.Matches(this.source.Text);

            foreach (Match match in matches)
            {
                result.Add(match.Value.TrimStart('[').TrimEnd(']'));
            }
            foreach (string tagname in result)
            {
                Tag tag = new Tag();
                tag.name = tagname;
                AddTag(tag);
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

            List<string> names = new List<string>();
            foreach (TagCreator tag in this.ContentPanel.Children)
            {
                if (names.Contains(tag.TagName.Text))
                {
                    ProgramState.ShowExclamationDialog($"Найдено несколько тегов с именем [{tag.TagName.Text}].\nНазвания тегов должны быть уникальными.", "Сохранение прервано");
                    return false;
                }
                names.Add(tag.TagName.Text);
            }


            return true;
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
    }
}
