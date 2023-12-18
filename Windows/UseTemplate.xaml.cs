using DocumentFormat.OpenXml.Office2021.PowerPoint.Designer;
using Incubator_2.Forms;
using Models;
using System.Collections.Generic;
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
        List<Tag> tags;
        public UseTemplate(Template t)
        {
            InitializeComponent();
            this.template = t;
            LoadTags();
            AddFileCreator();
            AddFileCreator();
        }
        private void LoadTags()
        {
            Tag t = new Tag();
            tags = t.GetAllTagsByTemplate(template.id);
        }
        
        private void AddFileCreator()
        {
            this.ContentPanel.Children.Add(new UC_FileCreator(tags));
        }

        private void CreateFileClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
