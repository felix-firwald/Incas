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
            creator.OnRemove += this.Creator_OnRemove;
            creator.OnMoveDownRequested += this.Creator_OnMoveDownRequested;
            creator.OnMoveUpRequested += this.Creator_OnMoveUpRequested;
            this.ContentPanel.Children.Add(creator);
        }

        private int Creator_OnMoveUpRequested(TableColumnCreator t)
        {
            int position = this.ContentPanel.Children.IndexOf(t);
            if (position < this.ContentPanel.Children.Count - 1)
            {
                position += 1;
            }
            this.ContentPanel.Children.Remove(t);
            this.ContentPanel.Children.Insert(position, t);
            return position;
        }

        private int Creator_OnMoveDownRequested(TableColumnCreator t)
        {
            int position = this.ContentPanel.Children.IndexOf(t);
            if (position > 0)
            {
                position -= 1;
            }

            this.ContentPanel.Children.Remove(t);
            this.ContentPanel.Children.Insert(position, t);
            return position;
        }

        private bool Creator_OnRemove(TableColumnCreator t)
        {
            this.ContentPanel.Children.Remove(t);
            return true;
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
            foreach (TableColumnCreator tcc in this.ContentPanel.Children)
            {
                tcc.Minimize();
            }
        }

        private void MaximizeAllClick(object sender, MouseButtonEventArgs e)
        {
            foreach (TableColumnCreator tcc in this.ContentPanel.Children)
            {
                tcc.Maximize();
            }
        }
    }
}
