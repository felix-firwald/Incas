using Incas.Objects.Components;
using Incas.Objects.Models;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using Incas.Objects.Views.Windows;
using System;
using Incas.Core.Classes;

namespace Incas.Objects.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ObjectsList.xaml
    /// </summary>
    public partial class ObjectsList : UserControl
    {
        public Class sourceClass;
        public ObjectsList(Class source)
        {
            this.InitializeComponent();
            this.sourceClass = source;
            this.UpdateView();
        }
        private void UpdateView()
        {
            DataTable dt = ObjectProcessor.GetObjectsList(this.sourceClass);
            this.Data.Columns.Clear();

            //foreach (DataColumn column in dt.Columns)
            //{
            //    DataGridTextColumn col = new();
            //    col.Header = column.ColumnName;
            //    if (column.ColumnName == ObjectProcessor.IdField)
            //    {
            //        col.Visibility = Visibility.Collapsed;
            //    }
            //    this.Data.Columns.Add(col);
                
            //}
            //foreach (DataRow row in dt.Rows)
            //{
            //    foreach (DataColumn column in dt.Columns)
            //    {
            //        this.Data.Items[row][d] 
            //    }
            //}
            this.Data.ItemsSource = dt.AsDataView();
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            ObjectsEditor oc = new(this.sourceClass);
            oc.Show();
        }

        private void CopyClick(object sender, RoutedEventArgs e)
        {

        }

        private void SearchClick(object sender, RoutedEventArgs e)
        {
            this.UpdateView();
        }

        private void FindBySelectionClick(object sender, RoutedEventArgs e)
        {

        }

        private void CancelSearchClick(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string source = ((DataRowView)this.Data.SelectedItems[0]).Row[ObjectProcessor.IdField].ToString();
            Guid id = Guid.Parse(source);
            Components.Object obj = ObjectProcessor.GetObject(this.sourceClass, id);
            ObjectsEditor oc = new(this.sourceClass, new() { obj });
            oc.Show();
        }
    }
}
