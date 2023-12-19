using Common;
using DocumentFormat.OpenXml.Office2021.PowerPoint.Designer;
using Incubator_2.Forms;
using Models;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;


namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для UseTemplate.xaml
    /// </summary>
    public partial class UseTemplate : Window
    {
        Template template;
        List<Tag> tags;
        List<UC_FileCreator> creators = new List<UC_FileCreator>();
        public UseTemplate(Template t)
        {
            InitializeComponent();
            this.template = t;
            LoadTags();
            AddFileCreator();
            this.dir.Text = RegistryData.GetTemplatePreferredPath(this.template.id.ToString());
        }
        private void LoadTags()
        {
            Tag t = new Tag();
            tags = t.GetAllTagsByTemplate(template.id);
        }
        
        private void AddFileCreator()
        {
            UC_FileCreator fc = new UC_FileCreator(tags);
            this.ContentPanel.Children.Add(fc);
            creators.Add(fc);
        }

        private bool ValidateContent()
        {
            if (!Directory.Exists(this.dir.Text))
            {
                ProgramState.ShowErrorDialog($"Папка, указанная в качестве выходной для генерации документов ({this.dir.Text}) не существует.\nУкажите существующую!", "Несуществующий выходной путь");
                return false;
            }
            return true;
        }

        private void CreateFilesClick(object sender, RoutedEventArgs e)
        {
            if (ValidateContent())
            {
                RegistryData.AddTemplate(this.template.id.ToString(), this.dir.Text, "", "");
                foreach (UC_FileCreator fc in creators)
                {
                    fc.CreateFile(this.dir.Text, template.path);
                }
            }
        }

        private void AddFC_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AddFileCreator();
        }

        private void review_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.RootFolder = System.Environment.SpecialFolder.MyDocuments;
            if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.dir.Text = fb.SelectedPath;
            }
        }
    }
}
