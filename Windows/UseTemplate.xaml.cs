using DocumentFormat.OpenXml.Office2021.PowerPoint.Designer;
using Models;
using System.Data;
using System.Windows;
using System.Windows.Controls;


namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для UseTemplate.xaml
    /// </summary>
    public partial class UseTemplate : Window
    {
        Template template;
        public UseTemplate(Template t)
        {
            InitializeComponent();
            this.template = t;
            
            
        }
        private void LoadTags()
        {
            Tag t = new Tag();
            t.GetAllTagsByTemplate(template.id);
        }

        private void CreateFileClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
