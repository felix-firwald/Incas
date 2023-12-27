using Common;
using Incubator_2.Common;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для UC_FileCreator.xaml
    /// </summary>
    public partial class UC_FileCreator : UserControl
    {
        private bool IsCollapsed = false;
        private int templateId;
        private
        List<Tag> tags;
        List<UC_TagFiller> TagFillers = new List<UC_TagFiller>();
        public UC_FileCreator(int template, List<Tag> tagsList)
        {
            InitializeComponent();
            this.tags = tagsList;
            this.templateId = template;
            FillContentPanel();
        }
        private void FillContentPanel()
        {
            this.tags.ForEach(t =>
            {
                UC_TagFiller tf = new UC_TagFiller(t);
                this.ContentPanel.Children.Add(tf);
                TagFillers.Add(tf);
            });
        }
        public void Maximize()
        {
            this.MainBorder.Height = this.ContentPanel.Height + 40;
            this.IsCollapsed = !this.IsCollapsed;
        }
        public void Minimize()
        {
            this.MainBorder.Height = 40;
            this.IsCollapsed = !this.IsCollapsed;
        }

        private void ResizeClick(object sender, MouseButtonEventArgs e)
        {
            if (this.IsCollapsed)
            {
                Maximize();
            }
            else
            {
                Minimize();
            }

        }
        private void Remove(object sender, MouseButtonEventArgs e)
        {
            Panel parentPanel = (Panel)this.Parent;
            parentPanel.Children.Remove(this);
        }
        private string RemoveUnresolvedChars(string input)
        {
            return input
                .Replace("/", "")
                .Replace("\\", "")
                .Replace(":", "")
                .Replace("?", "")
                .Replace("*", "")
                .Replace("<", "")
                .Replace(">", "")
                .Replace("|", "")
                .Replace("\"", "")
                .Trim();
        }
        public void CreateFile(string newPath, string original)
        {
            string newFile = $"{newPath}\\{RemoveUnresolvedChars(this.Filename.Text)}.docx";
            File.Copy(ProgramState.GetFullnameOfWordFile(original), newFile, true);
            WordTemplator wt = new WordTemplator(newFile);
            Dictionary<int, string> filledTags = new Dictionary<int, string>();
            foreach (UC_TagFiller tf in TagFillers)
            {
                string name = tf.GetTagName();
                int id = tf.GetId();
                string value = tf.GetValue();
                wt.Replace(name, value);
                filledTags.Add(id, value);
            }
            TemplateJSON tjson = new TemplateJSON(this.templateId, this.Filename.Text, filledTags);
            tjson.Convert();
        }
        public void RenameByTag(string tag, string prefix = "", string postfix = "")
        {
            string result = "";
            foreach (UC_TagFiller tf in TagFillers)
            {
                if (tf.GetTagName() == tag)
                {
                    result = tf.GetValue();
                    break;
                }
            }
            this.Filename.Text = $"{prefix}{result}{postfix}";
        }

        private void FontAwesome_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}
