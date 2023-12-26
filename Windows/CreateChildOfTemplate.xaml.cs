using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using Common;
using Incubator_2.Forms;
using Incubator_2.ViewModels;
using Models;

namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateChildOfTemplate.xaml
    /// </summary>
    public partial class CreateChildOfTemplate : Window
    {
        VM_ChildTemplate vm;
        public CreateChildOfTemplate(VM_ChildTemplate templ)
        {
            InitializeComponent();
            vm = templ;
            this.DataContext = templ;
            // загрузить обычные теги
            ParseParentTags();
            ParseChildTags();
        }

        private void ParseParentTags()
        {
            vm.parentTags.ForEach(parent =>    // для каждого тега родительского шаблона
            {
                NotOverridenTag not = new NotOverridenTag(parent);
                not.onOverride += AddOverriden;
                this.ParentTagPanel.Children.Add(not);
            });
        }
        private void ParseChildTags()
        {
            vm.childTags.ForEach(child =>    // для каждого тега родительского шаблона
            {
                TagCreator not = new TagCreator(child);
                not.onDelete += DeleteTagFromMain;
                this.ContentPanel.Children.Add(not);
            });
        }
        private void AddOverriden(Tag t)
        {
            Tag child = new Tag();
            child.parent = t.id;
            child.name = t.name;
            child.type = t.type;
            child.value = t.value;
            TagCreator tc = new TagCreator(child);
            tc.onDelete += DeleteTagFromMain;
            this.ContentPanel.Children.Add(tc);
        }

        private void DeleteTagFromMain(Tag t)
        {
            foreach (NotOverridenTag tagControl in this.ParentTagPanel.Children)
            {
                if (tagControl.tag.id == t.parent)
                {
                    tagControl.DeOverrideTag();
                    break;
                }
            }
        }

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

                    File.Copy(fd.FileName, $"{ProgramState.TemplatesSourcesWordPath}\\{fd.SafeFileName}");
                }
                result = fd.SafeFileName;
                this.vm.FilePath = result;
            }
        }

        private void AddTagClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.ContentPanel.Children.Add(new TagCreator(new Tag()));
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            this.vm.SaveTemplate();
            
            foreach (TagCreator tag in this.ContentPanel.Children)
            {
                tag.SaveTag(this.vm.childTemplate.id);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
