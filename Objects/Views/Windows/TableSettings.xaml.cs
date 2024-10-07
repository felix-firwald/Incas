using Incas.Objects.Components;
using Incas.Objects.Views.Controls;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace Incas.Objects.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для TableSettings.xaml
    /// </summary>
    public partial class TableSettings : Window
    {
        public TableFieldData Data { get; set; }
        public TableSettings(string value)
        {
            this.InitializeComponent();
            try
            {
                this.Data = JsonConvert.DeserializeObject<TableFieldData>(value);
                this.FillPanel(this.Data.Columns);
            }
            catch
            {
                this.Data = new();
                this.Data.Columns = new();
            }
        }
        public void FillPanel(List<TableFieldColumnData> columns)
        {
            this.ContentPanel.Children.Clear();
            foreach (TableFieldColumnData column in columns)
            {
                this.AddColumnCreator(column);
            }
        }
        private void AddColumnCreator(TableFieldColumnData col)
        {
            TableColumnCreator creator = new(col);
            this.ContentPanel.Children.Add(creator);
        }

        private void FinishClick(object sender, RoutedEventArgs e)
        {
            List<TableFieldColumnData> list = new();
            foreach (TableColumnCreator creator in this.ContentPanel.Children)
            {
                list.Add(creator.GetField());
            }
            this.Data.Columns = list;
            this.DialogResult = true;
        }

        private void AddFieldClick(object sender, MouseButtonEventArgs e)
        {
            this.AddColumnCreator(new());
        }

        private void MinimizeAllClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void MaximizeAllClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
