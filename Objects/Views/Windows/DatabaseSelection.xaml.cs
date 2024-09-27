using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Incas.Objects.Components;
using Incas.Objects.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using Query = Incas.Core.Classes.Query;

namespace Incas.Objects.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для DatabaseSelection.xaml
    /// </summary>
    public partial class DatabaseSelection : Window
    {
        public DialogStatus Result = DialogStatus.Undefined;
        public readonly BindingData Binding;
        public readonly Class Class;
        public Objects.Components.Object SelectedObject
        {
            get
            {
                return ObjectProcessor.GetObject(this.Class, this.SelectedId);
            }
        }
        private Guid SelectedId
        {
            get
            {
                try
                {
                    return Guid.Parse(((DataRowView)this.Grid.SelectedItems[0]).Row[ObjectProcessor.IdField].ToString());
                }
                catch
                {
                    return Guid.Empty;
                }
            }
        }

        public string SelectedValue => ((DataRowView)this.Grid.SelectedItems[0]).Row[ObjectProcessor.IdField].ToString();
        public DatabaseSelection(BindingData data, string custom = "")
        {
            this.InitializeComponent();
            this.Binding = data;
            this.Class = new(this.Binding.Class);
            this.Title = $"Выбор записи ({this.Class.name})";
            this.FillList(custom);
        }
        private void FillList(string custom)
        {
            DataTable dt = ObjectProcessor.GetObjectsList(this.Class);
            this.UpdateItemsSource(dt.Columns);
            this.Grid.ItemsSource = dt.DefaultView;
        }
        private void UpdateItemsSource(DataColumnCollection cols)
        {
            List<string> result = [];
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
        private void Grid_AutoGeneratingColumn(object sender, System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            Style style = this.FindResource("ColumnHeaderSpecial") as Style;
            if (e.Column.Header.ToString() == ObjectProcessor.IdField)
            {
                e.Column.Visibility = Visibility.Hidden;
            }
            else if (e.Column.Header.ToString() is ObjectProcessor.NameField)
            {
                e.Column.Header = "Наименование";
                e.Column.HeaderStyle = style;
                e.Column.MinWidth = 100;
                e.Column.CanUserReorder = false;
            }
            else if (e.Column.Header.ToString() is ObjectProcessor.DateCreatedField)
            {
                e.Column.Header = "Дата создания";
                e.Column.HeaderStyle = style;
                e.Column.MinWidth = 100;
                e.Column.MaxWidth = 120;
                e.Column.CanUserReorder = false;
            }
            else if (e.Column.Header.ToString() is ObjectProcessor.StatusField)
            {
                e.Column.Header = "Статус";
                e.Column.HeaderStyle = style;
                e.Column.MinWidth = 100;
                e.Column.MaxWidth = 180;
                e.Column.CanUserReorder = false;
            }
        }
        private void SelectClick(object sender, RoutedEventArgs e)
        {
            this.Finish();
        }
        private void Finish()
        {
            if (this.Grid.SelectedItems.Count == 0)
            {
                DialogsManager.ShowExclamationDialog("Нельзя использовать пустое значение!", "Значение не выбрано");
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
                this.SearchText.Text = this.SelectedValue;
            }
            catch { }
        }

        private void OnMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Finish();
        }
    }
}
