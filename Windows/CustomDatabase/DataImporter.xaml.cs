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
        VM_DataImporter vm;
        public DialogStatus Result = DialogStatus.Undefined;
        public DataTable ResultTable { get { return vm.Table; } }
        public DataImporter(DataTable dt)
        {
            InitializeComponent();
            vm = new(dt);
            this.DataContext = vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Result = DialogStatus.Yes;
            this.Close();
        }
    }
}
