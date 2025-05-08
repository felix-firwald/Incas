using Incas.DialogSimpleForm.Components;
using IncasEngine.AdditionalFunctionality;
using IncasEngine.ObjectiveEngine;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Incas.Core.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для TableViewer.xaml
    /// </summary>
    public partial class TableViewer : Window
    {
        public TableViewerSettings Settings { get; set; }
        public TableViewer(TableViewerSettings s)
        {
            this.InitializeComponent();
            this.Settings = s;
            this.Data.Columns.Clear();
            this.Data.ItemsSource = s.Table.AsDataView();
            this.Title = string.IsNullOrEmpty(s.Title) ? "<Таблица без имени>" : s.Title;
        }
        private void Data_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (this.Settings.IsColumnIsHidden(e.Column.Header.ToString()))
            {
                e.Column.Visibility = Visibility.Collapsed;
            }
        }
        private void OnMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
        }
    }
}
