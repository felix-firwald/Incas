using Common;
//using DocumentFormat.OpenXml.Wordprocessing;
using Incubator_2.Common;
using Incubator_2.Windows;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
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
using Windows.Devices.Geolocation;

namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для UC_FileCreator.xaml
    /// </summary>
    public partial class UC_FileCreator : UserControl
    {
        private bool IsCollapsed = false;
        private Template template;
        private List<Tag> tags;
        List<UC_TagFiller> TagFillers = new List<UC_TagFiller>();
        List<TableFiller> Tables = new List<TableFiller>();
        public UC_FileCreator(Template templ, List<Tag> tagsList)
        {
            InitializeComponent();
            this.tags = tagsList;
            this.template = templ;
            FillContentPanel();
        }
        private void FillContentPanel()
        {
            this.tags.ForEach(t =>
            {
                if (t.type != TypeOfTag.Table)
                {
                    UC_TagFiller tf = new UC_TagFiller(t);
                    this.ContentPanel.Children.Add(tf);
                    TagFillers.Add(tf);
                }
                else
                {
                    TableFiller tf = new TableFiller(t);
                    this.ContentPanel.Children.Add(tf);
                    Tables.Add(tf);
                }
            });
        }
        public void ApplyRecord(TemplateJSON record)
        {
            this.Filename.Text = record.file_name;
            foreach (KeyValuePair<int, string> tag in record.filled_tags)
            {
                foreach (UC_TagFiller tagfiller in TagFillers)
                {
                    if (tagfiller.tag.id == tag.Key)
                    {
                        tagfiller.SetValue(tag.Value);
                        break;
                    }
                }
            }
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
        public void CreateFile(string newPath)
        {
            string newFile = $"{newPath}\\{RemoveUnresolvedChars(this.Filename.Text)}.docx";
            File.Copy(ProgramState.GetFullnameOfWordFile(template.path), newFile, true);
            Dictionary<int, string> filledTags = new Dictionary<int, string>();
            WordTemplator wt = new WordTemplator(newFile);

            List<string> tagsToReplace = new List<string>();
            List<string> values = new List<string>();
            this.Dispatcher.Invoke(() =>
            {
                foreach (UC_TagFiller tf in TagFillers)
                {
                    string name = tf.GetTagName();
                    int id = tf.GetId();
                    string value = tf.GetValue();
                    filledTags.Add(id, value);
                    tagsToReplace.Add(name);
                    values.Add(value);
                }
                wt.Replace(tagsToReplace, values);
                foreach (TableFiller tab in Tables)
                {
                    wt.CreateTable(tab.tag.name, tab.DataTable);
                }
            });
            RegistreCreatedJSON.AddRecord(new TemplateJSON(this.template.id, this.template.name, this.Filename.Text, filledTags));
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
            this.Filename.Text = $"{prefix} {result} {postfix}".Trim();
        }

        private void PreviewCLick(object sender, MouseButtonEventArgs e)
        {
            string newFile = $"{ProgramState.TemplatesRuntime}\\{DateTime.Now.ToString("yyMMddHHmmssff")}.docx";
            File.Copy(ProgramState.GetFullnameOfWordFile(template.path), newFile, true);
            WordTemplator wt = new WordTemplator(newFile);

            List<string> tagsToReplace = new List<string>();
            List<string> values = new List<string>();
            foreach (UC_TagFiller tf in TagFillers)
            {
                string nameOf = tf.GetTagName();
                int id = tf.GetId();
                string value = tf.GetValue();
                tagsToReplace.Add(nameOf);
                values.Add(value);
            }
            wt.Replace(tagsToReplace, values);
            foreach (TableFiller tab in Tables)
            {
                wt.CreateTable(tab.tag.name, tab.DataTable);
            }
            string fileXPS = wt.TurnToXPS();
            PreviewWindow pr = new PreviewWindow(fileXPS);
            pr.ShowDialog();         
        }
    }
}
