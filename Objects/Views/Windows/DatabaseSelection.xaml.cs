using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Incas.Objects.Components;
using Incas.Objects.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows;

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
        public readonly ClassData ClassData;
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
        public Components.Object GetSelectedObject()
        {
            return ObjectProcessor.GetObject(this.Class, this.SelectedId);
        }

        public string SelectedValue => ((DataRowView)this.Grid.SelectedItems[0]).Row[ObjectProcessor.IdField].ToString();
        public DatabaseSelection(BindingData data)
        {
            this.InitializeComponent();
            this.Binding = data;
            this.Class = new(this.Binding.Class);
            this.ClassData = this.Class.GetClassData();
            if (this.ClassData is null || this.ClassData.Fields is null)
            {
                DialogsManager.ShowDatabaseErrorDialog("Не удалось идентифицировать класс и показать карту его объектов. Вероятно, это означает, что класс удален. Обратитесь к администратору рабочего пространства для устранения ошибки.", "Привязка сломана");
                this.IsEnabled = false;
                return;
            }
            this.Title = $"Выбор объекта ({this.Class.name})";
            this.SetFields();
            this.FillList();
        }
        private void FillList()
        {
            DataTable dt = ObjectProcessor.GetObjectsList(this.Class, null);
            this.UpdateItemsSource(dt.Columns);
            DataView dv = dt.AsDataView();
            if (this.ClassData.ClassType == ClassType.Model)
            {
                dv.Sort = $"[{ObjectProcessor.NameField}] ASC";
            }    
            this.Grid.ItemsSource = dv;
        }
        private void FillList(string field, string value)
        {
            DataTable dt = ObjectProcessor.GetObjectsListWhereLike(this.Class, null, field, value);
            this.UpdateItemsSource(dt.Columns);
            this.Grid.ItemsSource = dt.DefaultView;
        }
        private void SetFields()
        {
            List<string> fields = [];
            foreach (Models.Field field in this.ClassData.Fields)
            {
                fields.Add(field.VisibleName);
            }
            this.Fields.ItemsSource = fields;
        }
        private void UpdateItemsSource(DataColumnCollection cols)
        {
            List<string> result = [];
            foreach (DataColumn col in cols)
            {
                result.Add(col.ColumnName);
            }

            try
            {
                this.Fields.SelectedIndex = 0;
            }
            catch { }
        }
        private void Grid_AutoGeneratingColumn(object sender, System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            Style style = this.FindResource("ColumnHeaderSpecial") as Style;
            switch (e.Column.Header.ToString())
            {
                case ObjectProcessor.IdField:
                    e.Column.Visibility = Visibility.Hidden;
                    break;
                case ObjectProcessor.NameField:
                    e.Column.Header = "Наименование";
                    e.Column.HeaderStyle = style;
                    e.Column.MinWidth = 100;
                    e.Column.CanUserReorder = false;
                    break;
                case ObjectProcessor.DateCreatedField:
                    e.Column.Header = "Дата создания";
                    e.Column.HeaderStyle = style;
                    e.Column.MinWidth = 100;
                    e.Column.MaxWidth = 120;
                    e.Column.CanUserReorder = false;
                    break;
                case ObjectProcessor.StatusField:
                    e.Column.Visibility = Visibility.Hidden;
                    break;
                case ObjectProcessor.TargetClassField:
                case ObjectProcessor.TargetObjectField:
                    e.Column.Visibility = Visibility.Hidden;
                    break;
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
            this.FillList(this.Fields.SelectedValue?.ToString(), this.SearchText.Text);
        }

        private void ClearClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.SearchText.Text = "";
            this.FillList();
        }

        private void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (this.Grid.SelectedItems == null)
                {
                    return;
                }
                this.SearchText.Text = ((DataRowView)this.Grid.SelectedItems[0]).Row[this.Fields.SelectedValue.ToString()].ToString();
            }
            catch { }
        }

        private void OnMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Finish();
        }
    }
}
