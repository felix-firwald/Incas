using Common;
using IncasEngine.TemplateManager;
using Incubator_2.Common;
using Incubator_2.Forms;
using Incubator_2.Forms.Templates;
using Incubator_2.Models;
using Microsoft.Scripting.Hosting;
using Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

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
        private TemplateSettings templateSettings;
        private GeneratorMode generatorMode;
        private List<SGeneratedDocument> documentsData;
        public delegate void Base();
        public event Base OnFinishedEditing;
        private void Setup(Template templ, GeneratorMode gm)
        {
            this.InitializeComponent();
            this.generatorMode = gm;
            this.template = templ;
            this.Title = this.template.name;
            this.GetTags();
            this.templateSettings = this.template.GetTemplateSettings();
        }
        public UseTemplateText(Template templ, SGeneratedDocument data)
        {
            this.Setup(templ, GeneratorMode.OneForm);
            if (data.filledTags != null)
            {
                this.ApplyData(data);
            }
        }
        public UseTemplateText(Template templ, List<SGeneratedDocument> data)
        {
            this.Setup(templ, GeneratorMode.ManyForms);
            this.SuperElementsPanel.Visibility = Visibility.Visible;
            this.documentsData = data;
            this.AddElements(this.documentsData.Count);
        }
        private void AddElements(int count)
        {
            int primaryCount = this.ElementsPanel.Children.Count;
            for (int i = 1 + primaryCount; i <= count + primaryCount; i++)
            {
                RadioButton rb = new()
                {
                    IsChecked = i == 1,
                    Content = i.ToString(),
                    Style = this.FindResource("FormSelectorButton") as Style
                };
                rb.Checked += this.ElementSelected;
                this.ElementsPanel.Children.Add(rb);
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
        public List<SGeneratedDocument> GetData()
        {
            List<SGeneratedDocument> result = new()
            {
                id = this.template.id
            };
            List<SGeneratedTag> tags = new();
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
        private bool CustomValidate()
        {
            foreach (UC_TagFiller tf in this.ContentPanel.Children)
            {
                if (string.IsNullOrEmpty(tf.GetData()))
                {
                    ProgramState.ShowExclamationDialog($"Тег \"{tf.tag.name}\" не заполнен!", "Сохранение отклонено");
                    return false;
                }
            }
            try
            {
                bool result = true;
                
                if (!string.IsNullOrEmpty(this.templateSettings.Validation))
                {
                    ScriptScope scope = ScriptManager.GetEngine().CreateScope();
                    scope.SetVariable("result", true);
                    scope.SetVariable("fields", new List<string>());
                    scope.SetVariable("failed_text", "Текст не установлен.");
                    foreach (UC_TagFiller tf in this.ContentPanel.Children)
                    {
                        scope.SetVariable(tf.tag.name.Replace(" ", "_"), tf.GetData());
                    }
                    ScriptManager.Execute(this.templateSettings.Validation, scope);
                    result = scope.GetVariable("result");
                    if (!result)
                    {
                        ProgramState.ShowExclamationDialog(scope.GetVariable("failed_text"));
                        dynamic fields = scope.GetVariable("fields");
                        foreach (UC_TagFiller tf in this.ContentPanel.Children)
                        {
                            if (fields.Contains(tf.tag.name.Replace(" ", "_")))
                            {
                                tf.MarkAsNotValidated();
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog("При выполнении скрипта валидации возникла ошибка:\n" + ex.Message);
                return true;
            }
        }
        private void PlaySavingScript()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.templateSettings.OnSaving))
                {
                    ScriptScope scope = ScriptManager.GetEngine().CreateScope();
                    foreach (UC_TagFiller tf in this.ContentPanel.Children)
                    {
                        scope.SetVariable(tf.tag.name.Replace(" ", "_"), tf.GetData());
                    }
                    ScriptManager.Execute(this.templateSettings.OnSaving, scope);
                    foreach (UC_TagFiller tf in this.ContentPanel.Children)
                    {
                        if (tf.tag.type != TagType.Generator)
                        {
                            tf.SetValue(scope.GetVariable(tf.tag.name.Replace(" ", "_")));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog("При выполнении скрипта возникла ошибка:\n" + ex.Message);
            }
        }

        private void ApplyClick(object sender, RoutedEventArgs e)
        {
            if (!this.CustomValidate())
            {
                return;
            }
            this.PlaySavingScript();
            OnFinishedEditing?.Invoke();
            this.Result = DialogStatus.Yes;
            this.Close();
        }
        private void UpdateClick(object sender, RoutedEventArgs e)
        {
            this.ResultView.Text = this.GetText();
        }

        private void AddElementClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.AddElements(1);
        }

        private void ElementSelected(object sender, RoutedEventArgs e)
        {
            try
            {
                int number = int.Parse((sender as RadioButton).Content.ToString());
                if (this.documentsData[number].filledTags != null)
                {
                    this.ApplyData(this.documentsData[number]);
                }
            }
            catch { }
        }
    }
}
