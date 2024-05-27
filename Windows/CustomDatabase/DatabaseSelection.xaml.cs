using Common;
using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using Query = Incas.Core.Classes.Query;

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
                return ((DataRowView)this.Grid.SelectedItems[0]).Row[this.Field].ToString();
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
            this.FillList(custom);
        }
        private void FillList(string custom)
        {
            Query q = new("");
            q.typeOfConnection = DBConnectionType.CUSTOM;
            q.DBPath = ProgramState.GetFullPathOfCustomDb(this.Database);
            q.AddCustomRequest($"SELECT * FROM [{this.Table}] {custom}");
            DataTable dt = q.Execute();
            this.UpdateItemsSource(dt.Columns);
            this.Grid.ItemsSource = dt.DefaultView;
        }
        private void UpdateItemsSource(DataColumnCollection cols)
        {
            List<string> result = new();
            foreach (DataColumn col in cols)
            {
                result.Add(col.ColumnName);
            }
            this.Fields.ItemsSource = result;
            try
            {
                this.Fields.SelectedIndex = 0;
            }
            catch { }
        }

        private void SelectClick(object sender, RoutedEventArgs e)
        {
            if (Grid.SelectedItems.Count == 0)
            {
                ProgramState.ShowExclamationDialog("Нельзя использовать пустое значение!", "Значение не выбрано");
                return;
            }
            this.Result = DialogStatus.Yes;
            this.Close();
        }

        private void SearchClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.FillList($"WHERE [{this.Fields.SelectedValue}] LIKE '%{this.SearchText.Text}%'");
        }

        private void ClearClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.SearchText.Text = "";
            this.FillList("");
        }

        private void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                this.SearchText.Text = this.SelectedValues[this.Fields.SelectedValue.ToString()].ToString();
            }
            catch { }
        }
    }
}
