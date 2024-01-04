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
        public readonly STemplateJSON record;
        public delegate void SelectorNotify(FileCreated my);
        public event SelectorNotify OnSelectorChecked;
        public event SelectorNotify OnSelectorUnchecked;
        public FileCreated(STemplateJSON rec)
        {
            InitializeComponent();
            record = rec;
            this.Filename.Content = record.file_name;
            this.TemplateName.Content = record.template_name;
            this.GenerationTime.Content = record.generated_time.ToString("f");
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
                OnSelectorUnchecked(this);
                this.Selector.IsChecked = false;
            }
            else
            {
                OnSelectorChecked(this);
                this.Selector.IsChecked = true;
            }
        }
    }
}
