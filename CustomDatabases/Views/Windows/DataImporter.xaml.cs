using Incas.Core.Views.Windows;
using Incas.CustomDatabases.ViewModels;
using System.Data;
using System.Windows;

namespace Incas.CustomDatabases.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для DataImporter.xaml
    /// </summary>
    public partial class DataImporter : Window
    {
        private DataImporterViewModel vm;
        public DialogStatus Result = DialogStatus.Undefined;
        public DataTable ResultTable => this.vm.Table;
        public DataImporter(DataTable dt)
        {
            this.InitializeComponent();
            this.vm = new(dt);
            this.DataContext = this.vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Result = DialogStatus.Yes;
            this.Close();
        }
    }
}
