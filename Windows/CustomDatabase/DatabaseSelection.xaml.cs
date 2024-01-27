using Common;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
