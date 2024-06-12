using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Incas.CreatedDocuments.Models;
using Incas.Templates.Components;
using Incas.Templates.Models;
using Incas.Templates.Views.Controls;
using Incas.Templates.Views.Pages;
using Incubator_2.Common;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Incas.Templates.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для UseTemplateText.xaml
    /// </summary>
    public partial class UseTemplateText : Window
    {
        public DialogStatus Result = DialogStatus.Undefined;
        private Template template;
        private List<Tag> tags;
        private ElementCreator creator;
        public delegate void Base();
        public event Base OnFinishedEditing;
        public UseTemplateText(Template templ, GeneratedElement data)
        {
            this.InitializeComponent();
            this.template = templ;
            this.Title = this.template.name;
            this.GetTags();
            this.creator = new ElementCreator(this.template, this.tags);
            this.ContentPanel.Child = this.creator;
            if (data.filledTags != null)
            {
                this.ApplyData(data);
            }
        }
        private void GetTags()
        {
            using (Tag tag = new())
            {
                this.tags = tag.GetAllTagsByTemplate(this.template.id);
            }
        }

        public void ApplyData(GeneratedElement data)
        {
            this.creator.ApplyData(data);
        }
        public GeneratedElement GetData()
        {
            return this.creator.GetData();
        }
        public string GetText()
        {
            return this.creator.GetText();
        }

        private void ApplyClick(object sender, RoutedEventArgs e)
        {
            if (!this.creator.SimpleValidate())
            {
                return;
            }
            OnFinishedEditing?.Invoke();

            this.Result = DialogStatus.Yes;
            this.Close();
        }
        private void UpdateClick(object sender, RoutedEventArgs e)
        {
            this.ResultView.Text = this.GetText();
        }
    }
}
