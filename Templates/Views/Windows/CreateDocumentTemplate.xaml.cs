using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Incas.Objects.AutoUI;
using Incas.Objects.Components;
using Incas.Objects.Exceptions;
using Incas.Objects.Models;
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

        private bool RemoveTagFromList(Objects.Views.Controls.FieldCreator tag)
        {
            this.ContentPanel.Children.Remove(tag);
            return true;
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

        private List<Objects.Models.Field> GetTagsData()
        {
            List<Objects.Models.Field> tags = [];
            foreach (Objects.Views.Controls.FieldCreator tag in this.ContentPanel.Children)
            {
                tags.Add(tag.GetField());
            }
            return tags;
        }

        private void Window_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop)
                ? System.Windows.DragDropEffects.Copy
                : System.Windows.DragDropEffects.None;
        }

        private void Hyperlink_Click_1(object sender, RoutedEventArgs e)
        {
            if (DialogsManager.ShowQuestionDialog("Этот шаблон будет удален, а все его данные, " +
                "включая исходный файл, поля, наименование и категория автоматически будут перемещены в класс.", "Продолжить?", "Создать класс", "Отменить") == DialogStatus.Yes)
            {
                Template t = this.vm.GetTemplate();
                List<Objects.Models.Field> fields = t.GetFields();
                Class creation = new()
                {
                    category = t.suggestedPath,
                    name = t.name
                };
                ClassData cd = new()
                {
                    ClassType = Objects.Components.ClassType.Document,
                    Fields = fields
                };
                TemplateData td = new()
                {
                    File = t.path,
                    Name = t.name
                };
                cd.AddTemplate(td);
                creation.SetClassData(cd);
                creation.Save();
                this.Close();
                t.RemoveTemplate();              
            }          
        }
    }
}
