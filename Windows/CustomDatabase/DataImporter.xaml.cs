using Incas.Core.Views.Windows;
using Incubator_2.ViewModels.VM_CustomDB;
using System.Data;
using System.Windows;

namespace Incubator_2.Windows.CustomDatabase
{
    /// <summary>
    /// Логика взаимодействия для DataImporter.xaml
    /// </summary>
    public partial class DataImporter : Window
    {
        private VM_DataImporter vm;
        public DialogStatus Result = DialogStatus.Undefined;
        public DataTable ResultTable { get { return this.vm.Table; } }
        public DataImporter(DataTable dt)
        {
            InitializeComponent();
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
