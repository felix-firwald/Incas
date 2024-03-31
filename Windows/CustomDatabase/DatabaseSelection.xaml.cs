using Common;
using System.Data;
using System.Windows;
using Query = Common.Query;

namespace Incubator_2.Windows.CustomDatabase
{
    /// <summary>
    /// Логика взаимодействия для DatabaseSelection.xaml
    /// </summary>
    public partial class DatabaseSelection : Window
    {
        public DialogStatus Result = DialogStatus.Undefined;
        public readonly string Database;
        public readonly string Table;
        public readonly string Field;
        public string SelectedValue
        {
            get
            {
                return ((DataRowView)Grid.SelectedItems[0]).Row[Field].ToString();
            }
        }
        public DataRow SelectedValues
        {
            get
            {
                return ((DataRowView)Grid.SelectedItems[0]).Row;
            }
        }
        public DatabaseSelection(string database, string table, string field, string custom = "")
        {
            InitializeComponent();
            this.Database = database;
            this.Table = table;
            this.Title = $"Выбор записи ({table})";
            this.Field = field;
            FillList(custom);
        }
        private void FillList(string custom)
        {
            Query q = new("");
            q.typeOfConnection = DBConnectionType.CUSTOM;
            q.DBPath = ProgramState.GetFullPathOfCustomDb(Database);
            q.AddCustomRequest($"SELECT * FROM [{Table}] {custom}");
            DataTable dt = q.Execute();
            this.Grid.ItemsSource = dt.DefaultView;
        }

        private void SelectClick(object sender, RoutedEventArgs e)
        {
            if (Grid.SelectedItems.Count == 0)
            {
                ProgramState.ShowExclamationDialog("Нельзя использовать пустое значение!", "Значение не выбрано");
                return;
            }
            Result = DialogStatus.Yes;
            this.Close();
        }

    }
}
