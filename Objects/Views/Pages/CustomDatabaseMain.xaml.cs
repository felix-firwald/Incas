using Incas.CustomDatabases.ViewModels;
using Incas.Objects.ViewModels;

namespace Incas.Objects.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для CustomDatabaseMain.xaml
    /// </summary>
    public partial class CustomDatabaseMain : System.Windows.Controls.UserControl
    {
        public CustomDatabaseViewModel vm;
        public CustomDatabaseMain()
        {
            this.InitializeComponent();
            this.vm = new();
            this.ContentPanel.Content = new Core.Views.Controls.NoContent();
            this.vm.OnClassSelected += this.OnClassSelected;
            this.DataContext = this.vm;
        }
        public CustomDatabaseMain(string category)
        {
            this.InitializeComponent();
            this.vm = new()
            {
                SelectedCategory = category
            };
            
            this.vm.OnClassSelected += this.OnClassSelected;
            this.DataContext = this.vm;
        }

        private void OnClassSelected(Models.Class selectedClass)
        {
            if (selectedClass == null)
            {
                this.ContentPanel.Content = new Core.Views.Controls.NoContent();
                return;
            }
            this.ContentPanel.Content = new ObjectsList(selectedClass);
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.vm.UpdateAll();
        }
    }
}
