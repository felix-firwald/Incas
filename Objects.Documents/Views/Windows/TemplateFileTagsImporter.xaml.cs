using Incas.Objects.Documents.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace Incas.Objects.Documents.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для TemplateFileTagsAnalyzer.xaml
    /// </summary>
    public partial class TemplateFileTagsImporter : Window
    {
        public TemplateTagsAnalyzerViewModel vm { get; set; }
        public TemplateFileTagsImporter(List<string> items)
        {
            this.InitializeComponent();
            this.vm = new(items);
            this.DataContext = this.vm;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();            
        }
    }
}
