using Incas.Objects.Components;
using Incas.Objects.Models;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using Incas.Objects.Views.Windows;
using System;
using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using System.Collections.Generic;
using System.Windows.Media;
using System.Globalization;
using MailKit.Search;

namespace Incas.Objects.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ObjectsList.xaml
    /// </summary>
    public partial class ObjectsList : UserControl
    {
        public Class sourceClass;
        public ClassData ClassData;
        private ObjectCard ObjectCard;
        public ObjectsList(Class source)
        {
            this.InitializeComponent();         
            this.sourceClass = source;
            this.ClassData = source.GetClassData();
            //this.InitStyle();
            this.UpdateView();
            if (this.ClassData.ShowCard)
            {
                this.PlaceCard();
            }         
        }
        private void InitStyle()
        {
            Style style = new(typeof(DataGridRow), this.FindResource("RowMain") as Style);
            if (this.ClassData.Statuses is null)
            {
                return;
            }
            foreach (KeyValuePair<int, StatusData> status in this.ClassData.Statuses)
            {
                System.Windows.Data.Binding bind = new();
                bind.Source = ObjectProcessor.StatusField;
                DataTrigger trigger = new()
                {
                    Binding = bind,
                    
                    Value = status.Key.ToString(),
                };
                Setter setter = new()
                {
                    Property = DataGridRow.BackgroundProperty,
                    Value = new SolidColorBrush(status.Value.Color)
                };
                trigger.Setters.Add(setter);
                style.Triggers.Add(trigger);
            }
            this.Data.RowStyle = style;
            
        }

        private void PlaceCard()
        {
            this.ObjectCard = new(this.sourceClass);
            this.ObjectCard.OnFilterRequested += this.ObjectCard_OnFilterRequested;
            this.MainGrid.Children.Add(this.ObjectCard);
            Grid.SetRow(this.ObjectCard, 0);
            Grid.SetRowSpan(this.ObjectCard, 2);
            Grid.SetColumn(this.ObjectCard, 1);
            this.ObjectCard.SetEmpty();
        }

        private void ObjectCard_OnFilterRequested(FieldData data)
        {
            this.UpdateViewWithFilter(data);
        }

        private void UpdateView()
        {
            DataTable dt = ObjectProcessor.GetObjectsList(this.sourceClass);
            this.Data.Columns.Clear();
            this.Data.ItemsSource = dt.AsDataView();
        }
        private void UpdateViewWithSearch(FieldData data)
        {
            DataTable dt = ObjectProcessor.GetObjectsListWhereLike(this.sourceClass, data.ClassField.VisibleName, data.Value);
            this.Data.Columns.Clear();
            this.Data.ItemsSource = dt.AsDataView();
        }
        private void UpdateViewWithFilter(FieldData data)
        {
            DataTable dt = ObjectProcessor.GetObjectsListWhereEqual(this.sourceClass, data.ClassField.VisibleName, data.Value);
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
                e.Column.MaxWidth = 180;
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
                //e.Column.Header = "Статус";
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
            this.UpdateView();
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
            ObjectsEditor oc = new(this.sourceClass, new List<Components.Object>() { obj });
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
            obj.AuthorId = Guid.Empty;
            obj.Status = 0;
            ObjectsEditor oc = new (this.sourceClass, new List<Components.Object>() { obj });
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
            Components.Object obj = ObjectProcessor.GetObject(this.sourceClass, id);
            if (this.ClassData.EditByAuthorOnly == true && obj.AuthorId != ProgramState.CurrentUser.id)
            {
                DialogsManager.ShowAccessErrorDialog("Этот объект не может быть удален, поскольку вы не являетесь его автором.");
                return;
            }
            if (DialogsManager.ShowQuestionDialog(
                $"Вы действительно хотите удалить объект \"{obj.Name}\" из базы данных?", 
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

        private void Data_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ClassData.ShowCard)
            {
                Guid id = this.GetSelectedObjectGuid();
                if (id == Guid.Empty)
                {
                    this.ObjectCard.SetEmpty();
                }
                else
                {
                    this.ObjectCard.UpdateFor(ObjectProcessor.GetObject(this.sourceClass, id));
                }         
            }
        }

        private void OpenInAnotherWindowClick(object sender, RoutedEventArgs e)
        {
            ContainerWindow cw = new(this, this.sourceClass.name);
            cw.Show();
        }

        
    }
}
