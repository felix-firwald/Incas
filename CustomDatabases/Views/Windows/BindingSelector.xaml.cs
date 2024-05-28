using Incas.Core.Views.Windows;
using Incas.CustomDatabases.ViewModels;
using System.Windows;

namespace Incas.CustomDatabases.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для BindingSelector.xaml
    /// </summary>
    public partial class BindingSelector : Window
    {
        private BindingSelectorViewModel vm;
        public DialogStatus Result = DialogStatus.Undefined;
        public string SelectedDatabase { get { return this.vm.SelectedDatabase.path; } }
        public string SelectedTable { get { return this.vm.SelectedTable; } }
        public string SelectedField { get { return this.vm.SelectedField; } }
        public BindingSelector()
        {
            this.InitializeComponent();
            this.vm = new();
            this.DataContext = this.vm;
        }
        public BindingSelector(string database, string table, bool dbEnabled = true, bool tableEnabled = true)
        {
            this.InitializeComponent();
            this.vm = new();
            this.DataContext = this.vm;
            this.vm.SetSelectedDatabase(database);
            this.vm.SelectedTable = table;
            this.vm.DatabaseSelectionEnable = dbEnabled;
            this.vm.TableSelectionEnable = tableEnabled;
        }
        public BindingSelector(string database, bool dbEnabled = true)
        {
            this.InitializeComponent();
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
