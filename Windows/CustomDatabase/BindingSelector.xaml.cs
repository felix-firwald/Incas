using Incubator_2.ViewModels.VM_CustomDB;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для BindingSelector.xaml
    /// </summary>
    public partial class BindingSelector : Window
    {
        VM_BindingSelector vm;
        public DialogStatus Result = DialogStatus.Undefined;
        public string SelectedDatabase { get { return vm.SelectedDatabase.path; } }
        public string SelectedTable { get { return vm.SelectedTable; } }
        public string SelectedField { get { return vm.SelectedField; } }
        public BindingSelector()
        {
            InitializeComponent();
            vm = new();
            this.DataContext = vm;
        }
        public BindingSelector(string database, string table, bool dbEnabled = true, bool tableEnabled = true)
        {
            InitializeComponent();
            vm = new();
            this.DataContext = vm;
            vm.SetSelectedDatabase(database);
            vm.SelectedTable = table;
            vm.DatabaseSelectionEnable = dbEnabled;
            vm.TableSelectionEnable = tableEnabled;
        }
        public BindingSelector(string database, bool dbEnabled = true)
        {
            InitializeComponent();
            vm = new();
            this.DataContext = vm;
            vm.SetSelectedDatabase(database);
            vm.DatabaseSelectionEnable = dbEnabled;
        }

        private void SelectClick(object sender, RoutedEventArgs e)
        {
            if (vm.ValidateContent())
            {
                Result = DialogStatus.Yes;
                this.Close();
            }
        }
    }
}
