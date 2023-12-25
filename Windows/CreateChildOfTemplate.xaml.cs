using System.Windows;
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
        private void AddOverriden(Tag t)
        {
            Tag child = new Tag();
            child.parent = t.id;
            child.name = t.name;
            child.type = t.type;
            child.value = t.value;
            this.ContentPanel.Children.Add(new TagCreator(child));
        }

        private void reviewClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
