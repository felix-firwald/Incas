using Common;
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
        private List<string> ChildsChoice = new List<string>();
        List<Tag> tags;
        List<UC_TagFiller> TagFillers = new List<UC_TagFiller>();
        public UC_FileCreator(List<Tag> tagsList, List<string> childs = null)
        {
            InitializeComponent();
            this.tags = tagsList;
            FillContentPanel();
            this.ChildChoicer.ItemsSource = childs;
            this.ChildChoicer.SelectedIndex = 0;
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
                .Replace("\"", "");
        }
        public void CreateFile(string newPath, string original)
        {
            string newFile = $"{newPath}\\{this.Filename.Text}.docx";
            File.Copy(ProgramState.GetFullnameOfWordFile(original), newFile, true);
            WordTemplator wt = new WordTemplator(newFile);
            foreach (UC_TagFiller tf in TagFillers)
            {
                wt.Replace(tf.GetTagName(), tf.GetValue());
            }
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
    }
}
