using Common;
using Incubator_2.Forms;
using Models;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Forms;


namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateTemplateWord.xaml
    /// </summary>
    public partial class CreateTemplateWord : Window
    {
        Template template;
        private readonly bool isEdit = false;

        public CreateTemplateWord(Template te = null)
        {
            InitializeComponent();
            if (te == null)
            {
                template = new Template();
            }
            else
            {
                isEdit = true;
                template = te;
                GetTags();
            }
            this.DataContext = template;
        }

        private void GetTags()
        {
            Tag tag = new Tag();
            foreach(Tag t in tag.GetAllTagsByTemplate(template.id))
            {
                AddTag(t);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы", Justification = "<Ожидание>")]
        private void reviewClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "MS Word|*.docx";
            fd.InitialDirectory = ProgramState.TemplatesSourcesWordPath;
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string result;
                if (!fd.FileName.StartsWith(ProgramState.TemplatesSourcesWordPath))
                {
                    
                    result = $"{ProgramState.TemplatesSourcesWordPath}\\{fd.SafeFileName}";
                    File.Copy(fd.FileName, result);
                }
                else
                {
                    result = fd.FileName;
                }
                this.sourceFile.Text = result;
            }
        }

        private bool CheckForSave()
        {
            return true;
        }

        

        private void saveClick(object sender, RoutedEventArgs e)
        {
            ProgramState.ShowErrorDialog($"name: {this.template.name}\n" +
                $"suggestedPath: {this.template.suggestedPath}\n" +
                $"file: {this.template.path}", "Вот данные:");
        }

        private void AddTag(Tag tag = null)
        {
            Tag t = new Tag();
            if (tag == null)
            {
                t.name = "Новый";
            }
            else
            {
                t = tag;
            }
            this.ContentPanel.Children.Add(new TagCreator(t));
        }

        private void AddTagClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AddTag();
        }
    }
}
