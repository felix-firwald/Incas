using Incubator_2.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для FileCreated.xaml
    /// </summary>
    public partial class FileCreated : UserControl
    {
        public readonly SGeneratedDocument record;
        public delegate void SelectorNotify(FileCreated my);
        public event SelectorNotify OnSelectorChecked;
        public event SelectorNotify OnSelectorUnchecked;
        public FileCreated(SGeneratedDocument rec, int counter = 1)
        {
            InitializeComponent();
            record = rec;
            this.Counter.Content = counter;
            this.Filename.Content = record.fileName;
            this.TemplateName.Text = record.templateName;
            this.GenerationTime.Content = record.generatedTime.ToString("f");
        }

        private void Selector_Checked(object sender, RoutedEventArgs e)
        {
            OnSelectorChecked(this);
        }

        private void Selector_Unchecked(object sender, RoutedEventArgs e)
        {
            OnSelectorUnchecked(this);
        }

        private void OnClick(object sender, MouseButtonEventArgs e)
        {
            if (this.Selector.IsChecked == true)
            {
                this.Selector.IsChecked = false;
            }
            else
            {
                this.Selector.IsChecked = true;
            }
        }
    }
}
