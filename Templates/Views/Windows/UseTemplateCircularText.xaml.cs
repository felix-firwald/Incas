using Incas.Core.Views.Windows;
using Incas.CreatedDocuments.Models;
using Incas.Templates.Models;
using Incas.Templates.Views.Pages;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Incas.Templates.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для UseTemplateCircularText.xaml
    /// </summary>
    public partial class UseTemplateCircularText : Window
    {
        public DialogStatus Result = DialogStatus.Undefined;
        private Template template;
        private List<Tag> tags;
        private bool autoUpdating;
        public UseTemplateCircularText(Template templ, List<GeneratedElement> data)
        {
            this.InitializeComponent();
            this.template = templ;
            this.GetTags();
            this.ApplyData(data);
            this.autoUpdating = true;
            this.Updating();
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
            this.FillerText.Text = data[0].filler;
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
            GeneratedElement ge = result[0];
            ge.filler = this.FillerText.Text;
            result[0] = ge;
            return result;
        }
        public string GetText()
        {
            List<string> result = new();
            foreach (ElementCreator ec in this.ContentPanel.Children)
            {
                result.Add(ec.GetText());
            }
            return string.Join(this.FillerText.Text, result);
        }

        private void AddElement()
        {
            ElementCreator ec = new(this.template, this.tags);
            ec.OnCreatorDestroy += this.Element_OnCreatorDestroy;
            this.ContentPanel.Children.Add(ec);
        }

        private void Element_OnCreatorDestroy(ElementCreator creator)
        {
            this.ContentPanel.Children.Remove(creator);
        }

        private void AddElement(GeneratedElement doc)
        {
            ElementCreator ec = new(this.template, this.tags);
            ec.ApplyData(doc);
            this.ContentPanel.Children.Add(ec);
        }
        private async void Updating()
        {
            await Task.Run(() =>
            {
                while (this.autoUpdating)
                {
                    Thread.Sleep(1000);
                    this.Dispatcher.Invoke(() =>
                    {
                        this.UpdateView();
                    });                
                }
            });
        }
        private void UpdateView()
        {
            this.FlowDocument.Blocks.Clear();
            Paragraph paragraph = new()
            {
                Foreground = new SolidColorBrush(System.Windows.Media.Colors.White)
            };
            
            uint index = 0;
            foreach (ElementCreator ec in this.ContentPanel.Children)
            {              
                Run r = new(ec.GetText());
                if (index > 0)
                {
                    Run filler = new(this.FillerText.Text)
                    {
                        Foreground = new SolidColorBrush(System.Windows.Media.Colors.Lime)
                    };
                    paragraph.Inlines.Add(filler);
                }
                index++;               
                paragraph.Inlines.Add(r);
            }
            this.FlowDocument.Blocks.Add(paragraph);
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
            this.Result = DialogStatus.Yes;
            this.Close();
        }

        private void OnClosingWanted(object sender, System.EventArgs e)
        {
            this.autoUpdating = false;
        }
    }
}
