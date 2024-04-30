using Incubator_2.ViewModels.VM_CustomDB;
using System.Windows;

namespace Incubator_2.Windows.CustomDatabase
{
    /// <summary>
    /// Логика взаимодействия для BindingSelector.xaml
    /// </summary>
    public partial class BindingSelector : Window
    {
        private VM_BindingSelector vm;
        public DialogStatus Result = DialogStatus.Undefined;
        public string SelectedDatabase { get { return this.vm.SelectedDatabase.path; } }
        public string SelectedTable { get { return this.vm.SelectedTable; } }
        public string SelectedField { get { return this.vm.SelectedField; } }
        public BindingSelector()
        {
            InitializeComponent();
            this.vm = new();
            this.DataContext = this.vm;
        }
        public BindingSelector(string database, string table, bool dbEnabled = true, bool tableEnabled = true)
        {
            InitializeComponent();
            this.vm = new();
            this.DataContext = this.vm;
            this.vm.SetSelectedDatabase(database);
            this.vm.SelectedTable = table;
            this.vm.DatabaseSelectionEnable = dbEnabled;
            this.vm.TableSelectionEnable = tableEnabled;
        }
        public BindingSelector(string database, bool dbEnabled = true)
        {
            InitializeComponent();
            this.vm = new();
            this.DataContext = this.vm;
            this.vm.SetSelectedDatabase(database);
            this.vm.DatabaseSelectionEnable = dbEnabled;
        }

        private void SelectClick(object sender, RoutedEventArgs e)
        {
            if (this.vm.ValidateContent())
            {
                this.Result = DialogStatus.Yes;
                this.Close();
            }
        }
    }
}
