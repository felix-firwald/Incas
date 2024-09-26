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
            this.vm.OnClassSelected += this.OnClassSelected;
            this.DataContext = this.vm;
        }

        private void OnClassSelected(Models.Class selectedClass)
        {
            if (selectedClass == null)
            {
                return;
            }
            this.ContentPanel.Content = new ObjectsList(selectedClass);
        }
    }
}
