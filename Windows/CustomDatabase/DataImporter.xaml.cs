using Incubator_2.ViewModels;
using Incubator_2.ViewModels.VM_CustomDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
