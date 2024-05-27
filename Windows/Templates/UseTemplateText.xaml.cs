using Common;
using Incas.Core.Views.Windows;
using IncasEngine.TemplateManager;
using Incubator_2.Common;
using Incubator_2.Forms;
using Incubator_2.Models;
using Microsoft.Scripting.Hosting;
using Models;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Incubator_2.Windows.Templates
{
    /// <summary>
    /// Логика взаимодействия для UseTemplateText.xaml
    /// </summary>
    public partial class UseTemplateText : Window
    {
        public DialogStatus Result = DialogStatus.Undefined;
        private Template template;
        private List<Tag> tags;
        public delegate void Base();
        public event Base OnFinishedEditing;
        public UseTemplateText(Template templ, SGeneratedDocument data)
        {
            this.InitializeComponent();
            this.template = templ;
            this.Title = this.template.name;
            this.GetTags();
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
            foreach (Tag tag in this.tags)
            {
                this.AddField(tag);
            }
        }
        private void AddField(Tag t)
        {
            UC_TagFiller tf = new(t);
            tf.OnScriptRequested += this.OnScriptRequested;
            this.ContentPanel.Children.Add(tf);
        }

        private void OnScriptRequested(string script)
        {
            try
            {
                ScriptScope scope = ScriptManager.GetEngine().CreateScope();
                foreach (UC_TagFiller tf in this.ContentPanel.Children)
                {
                    scope.SetVariable(tf.tag.name.Replace(" ", "_"), tf.GetData());
                }
                ScriptManager.Execute(script, scope);
                foreach (UC_TagFiller tf in this.ContentPanel.Children)
                {
                    tf.SetValue(scope.GetVariable(tf.tag.name.Replace(" ", "_")));
                }
            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog("При обработке скрипта на стороне формы возникла ошибка:\n" + ex.Message);
            }
        }

        public void ApplyData(SGeneratedDocument data)
        {
            foreach (SGeneratedTag tag in data.GetFilledTags())
            {
                foreach (UC_TagFiller tagfiller in this.ContentPanel.Children)
                {
                    if (tagfiller.tag.id == tag.tag)
                    {
                        tagfiller.SetValue(tag.value);
                        break;
                    }
                }
            }
        }
        public SGeneratedDocument GetData()
        {
            SGeneratedDocument result = new()
            {
                id = this.template.id
            };
            List<SGeneratedTag> tags = [];
            foreach (UC_TagFiller tf in this.ContentPanel.Children)
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
            foreach (UC_TagFiller tf in this.ContentPanel.Children)
            {
                result = result.Replace($"[{tf.tag.name}]", tf.GetValue());
            }
            return result;
        }

        private void ApplyClick(object sender, RoutedEventArgs e)
        {
            foreach (UC_TagFiller tf in this.ContentPanel.Children)
            {
                if (string.IsNullOrEmpty(tf.GetData()))
                {
                    ProgramState.ShowExclamationDialog($"Тег \"{tf.tag.name}\" не заполнен!", "Сохранение отклонено");
                    return;
                }
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
