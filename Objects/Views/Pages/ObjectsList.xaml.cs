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
            this.Data.ItemsSource = dt.AsDataView();
        }
        private void Data_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
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
        private void AddClick(object sender, RoutedEventArgs e)
        {
            ObjectsEditor oc = new(this.sourceClass);
            oc.OnUpdateRequested += this.ObjectsEditor_OnUpdateRequested;
            oc.Show();
        }

        private void CopyClick(object sender, RoutedEventArgs e)
        {
            this.OpenCopyOfSelectedObject();
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
            this.RemoveSelectedObject();
            this.UpdateView();
        }
        private Guid GetSelectedObjectGuid()
        {
            if (this.Data.SelectedItems.Count == 0)
            {
                return Guid.Empty;
            }
            string source = ((DataRowView)this.Data.SelectedItems[0]).Row[ObjectProcessor.IdField].ToString();
            return Guid.Parse(source);
        }
        private void OpenSelectedObject()
        {
            Guid id = this.GetSelectedObjectGuid();
            if (id == Guid.Empty)
            {
                return;
            }
            Components.Object obj = ObjectProcessor.GetObject(this.sourceClass, id);
            ObjectsEditor oc = new(this.sourceClass, new() { obj });
            oc.OnUpdateRequested += this.ObjectsEditor_OnUpdateRequested;
            oc.Show();
        }

        private void OpenCopyOfSelectedObject()
        {
            Guid id = this.GetSelectedObjectGuid();
            if (id == Guid.Empty)
            {
                return;
            }
            Components.Object obj = ObjectProcessor.GetObject(this.sourceClass, id);
            obj.Id = Guid.Empty;
            ObjectsEditor oc = new(this.sourceClass, new() { obj });
            oc.OnUpdateRequested += this.ObjectsEditor_OnUpdateRequested;
            oc.Show();
        }
        private void ObjectsEditor_OnUpdateRequested()
        {
            this.UpdateView();
        }
        private void RemoveSelectedObject()
        {
            Guid id = this.GetSelectedObjectGuid();
            if (id == Guid.Empty)
            {
                return;
            }
            if (DialogsManager.ShowQuestionDialog(
                $"Вы действительно хотите удалить объект \"{ObjectProcessor.GetObject(this.sourceClass, id).Name}\" из базы данных?", 
                "Удалить?", 
                "Удалить", 
                "Не удалять") == Core.Views.Windows.DialogStatus.Yes)
            {
                ObjectProcessor.RemoveObject(this.sourceClass, id);
            }           
        }
        private void OnMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.OpenSelectedObject();
        }

        private void Data_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.Enter:
                    this.OpenSelectedObject();
                    break;
                case System.Windows.Input.Key.C:
                case System.Windows.Input.Key.RightShift:
                    this.OpenCopyOfSelectedObject();
                    break;
                case System.Windows.Input.Key.R:
                case System.Windows.Input.Key.Delete:
                    this.RemoveSelectedObject();
                    this.UpdateView();
                    break;
                case System.Windows.Input.Key.U:
                case System.Windows.Input.Key.Space:
                    this.UpdateView();
                    break;

            }
            
        }

    }
}
