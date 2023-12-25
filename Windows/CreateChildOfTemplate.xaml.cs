using System.Windows;
using Incubator_2.ViewModels;
using Models;

namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateChildOfTemplate.xaml
    /// </summary>
    public partial class CreateChildOfTemplate : Window
    {
        public CreateChildOfTemplate(VM_ChildTemplate templ)
        {
            InitializeComponent();
            this.DataContext = templ;
        }

        private void reviewClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
