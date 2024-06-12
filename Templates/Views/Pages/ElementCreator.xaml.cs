using Incas.Core.Classes;
using Incas.CreatedDocuments.Models;
using Incas.Templates.Components;
using Incas.Templates.Models;
using Incas.Templates.Views.Controls;
using Incubator_2.Common;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incas.Templates.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ElementCreator.xaml
    /// </summary>
    public partial class ElementCreator : UserControl
    {
        private Template template;
        private List<Tag> tags;

        public delegate void ElementCreatorAction(ElementCreator creator);
        public event ElementCreatorAction OnCreatorDestroy;

        public delegate void TagActionRecalculate(string tag);
        public ElementCreator(Template template, List<Tag> tags)
        {
            this.InitializeComponent();
            this.template = template;
            this.tags = tags;
            this.FillPanel();
            this.ExpanderButton.IsChecked = true;
        }
        private void FillPanel()
        {
            foreach (Tag tag in this.tags)
            {
                this.AddField(tag);
            }
        }
        private void AddField(Tag t)
        {
            TagFiller tf = new(t);
            tf.OnScriptRequested += this.OnScriptRequested;
            this.ContentPanel.Children.Add(tf);
        }
        public void ApplyData(GeneratedElement data)
        {
            if (data.filledTags != null)
            {
                foreach (SGeneratedTag tag in data.filledTags)
                {
                    foreach (TagFiller tagfiller in this.ContentPanel.Children)
                    {
                        if (tagfiller.tag.id == tag.tag)
                        {
                            tagfiller.SetValue(tag.value);
                            break;
                        }
                    }
                }
            }         
        }
        public GeneratedElement GetData()
        {
            GeneratedElement result = new()
            {
                template = this.template.id
            };
            List<SGeneratedTag> tags = [];
            foreach (TagFiller tf in this.ContentPanel.Children)
            {
                SGeneratedTag gt = new()
                {
                    tag = tf.tag.id,
                    value = tf.tag.type == TagType.Generator ? tf.GetData() : tf.GetValue()
                };
                tags.Add(gt);
            }
            result.filledTags = tags;
            return result;
        }
        public string GetText()
        {
            string result = this.template.path;
            foreach (TagFiller tf in this.ContentPanel.Children)
            {
                result = result.Replace($"[{tf.tag.name}]", tf.GetValue());
            }
            return result;
        }

        private void OnScriptRequested(string script)
        {
            try
            {
                ScriptScope scope = ScriptManager.GetEngine().CreateScope();
                foreach (TagFiller tf in this.ContentPanel.Children)
                {
                    scope.SetVariable(tf.tag.name.Replace(" ", "_"), tf.GetData());
                }
                ScriptManager.Execute(script, scope);
                foreach (TagFiller tf in this.ContentPanel.Children)
                {
                    tf.SetValue(scope.GetVariable(tf.tag.name.Replace(" ", "_")));
                }
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex, "При обработке скрипта на стороне формы возникла ошибка");
            }
        }
        public bool SimpleValidate()
        {
            foreach (TagFiller tf in this.ContentPanel.Children)
            {
                if (string.IsNullOrEmpty(tf.GetData()))
                {
                    DialogsManager.ShowExclamationDialog($"Тег \"{tf.tag.name}\" не заполнен!", "Сохранение отклонено");
                    return false;
                }
            }
            return true;
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
    }
}
