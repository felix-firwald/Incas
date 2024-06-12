using Incas.CreatedDocuments.Models;
using Incas.Templates.Models;
using Incas.Templates.Views.Pages;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Incas.Templates.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для UseTemplateCircularText.xaml
    /// </summary>
    public partial class UseTemplateCircularText : Window
    {
        private Template template;
        private List<Tag> tags;
        public UseTemplateCircularText(Template templ, List<GeneratedElement> data)
        {
            this.InitializeComponent();
            this.template = templ;
            this.GetTags();
            this.ApplyData(data);
        }
        private void GetTags()
        {
            using (Tag tag = new())
            {
                this.tags = tag.GetAllTagsByTemplate(this.template.id);
            }
        }

        public void ApplyData(List<GeneratedElement> data)
        {
            this.ContentPanel.Children.Clear();
            foreach (GeneratedElement el in data)
            {
                this.AddElement(el);
            }
        }
        public List<GeneratedElement> GetData()
        {
            List<GeneratedElement> result = new();
            foreach (ElementCreator ec in this.ContentPanel.Children)
            {
                result.Add(ec.GetData());
            }
            return result;
        }
        public string GetText()
        {
            List<string> result = new();
            foreach (ElementCreator ec in this.ContentPanel.Children)
            {
                result.Add(ec.GetText());
            }
            return string.Join("\n", result);
        }

        private void AddElement()
        {
            ElementCreator ec = new(this.template, this.tags);
            this.ContentPanel.Children.Add(ec);
        }
        private void AddElement(GeneratedElement doc)
        {
            ElementCreator ec = new(this.template, this.tags);
            ec.ApplyData(doc);
            this.ContentPanel.Children.Add(ec);
        }

        private void AddElementClick(object sender, MouseButtonEventArgs e)
        {
            this.AddElement();
        }

        private void GetFromExcelClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void SendToExcelClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void GenerateClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
