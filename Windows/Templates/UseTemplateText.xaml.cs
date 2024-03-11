using Incubator_2.Forms;
using Incubator_2.Models;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            InitializeComponent();
            template = templ;
            this.Title = template.name;
            GetTags();
            if (data.filledTags != null)
            {
                ApplyData(data);
            }            
        }
        private void GetTags()
        {
            using (Tag tag = new())
            {
                this.tags = tag.GetAllTagsByTemplate(template.id);
            }
            foreach (Tag tag in this.tags)
            {
                AddField(tag);
            }
        }
        private void AddField(Tag t)
        {
            UC_TagFiller tf = new(t);
            this.ContentPanel.Children.Add(tf);
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
            SGeneratedDocument result = new();
            result.id = template.id;
            List<SGeneratedTag> tags = new();
            foreach (UC_TagFiller tf in this.ContentPanel.Children)
            {
                SGeneratedTag gt = new();
                gt.tag = tf.tag.id;
                if (tf.tag.type == TypeOfTag.Generator)
                {
                    gt.value = tf.GetData();
                }
                else
                {
                    gt.value = tf.GetValue();
                } 
                tags.Add(gt);
            }
            result.filledTags = tags;
            return result;
        }
        public string GetText()
        {
            string result = template.path;
            foreach (UC_TagFiller tf in this.ContentPanel.Children)
            {
                result = result.Replace($"[{tf.tag.name}]", tf.GetValue());
            }
            return result;
        }

        private void ApplyClick(object sender, RoutedEventArgs e)
        {
            if (OnFinishedEditing != null)
            {
                OnFinishedEditing();
            }
            
            Result = DialogStatus.Yes;
            this.Close();
        }
        private void UpdateClick(object sender, RoutedEventArgs e)
        {
            this.ResultView.Text = GetText();
        }
    }
}
