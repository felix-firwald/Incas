using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Incas.Objects.AutoUI;
using Incas.Objects.Components;
using Incas.Objects.Models;
using Incas.Objects.Views.Windows;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Incas.Objects.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ObjectsList.xaml
    /// </summary>
    public partial class ObjectsList : UserControl
    {
        private const string ColorColumn = "__COLOR_COLUMN__";
        public Class sourceClass;
        public ClassData ClassData;
        private ObjectCard ObjectCard;
        public Preset SourcePreset;
        public ObjectsList(Class source)
        {
            this.InitializeComponent();
            DialogsManager.ShowWaitCursor();
            this.sourceClass = source;
            this.ClassData = source.GetClassData();
            this.UpdateView();
            if (this.ClassData.ShowCard)
            {
                this.PlaceCard();
            }
            DialogsManager.ShowWaitCursor(false);
        }
        public ObjectsList(Class source, Preset preset)
        {
            this.InitializeComponent();
            DialogsManager.ShowWaitCursor();
            this.sourceClass = source;
            this.ClassData = source.GetClassData();
            this.UpdateView();
            if (this.ClassData.ShowCard)
            {
                this.PlaceCard();
            }
            this.SourcePreset = preset;
            DialogsManager.ShowWaitCursor(false);
        }

        private DataTable WrapWithStyle(DataTable dt)
        {
            if (dt == null || dt.Rows == null)
            {
                return dt;
            }
            dt.Columns.Add(ColorColumn);
            foreach (DataRow row in dt.Rows)
            {
                int n = int.Parse(row[ObjectProcessor.StatusField].ToString());
                row[ColorColumn] = this.ClassData.Statuses[n].Color;
            }
            return dt;
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
                System.Windows.Data.Binding bind = new()
                {
                    Source = ObjectProcessor.StatusField
                };
                DataTrigger trigger = new()
                {
                    Binding = bind,

                    Value = status.Key.ToString(),
                };
                Setter setter = new()
                {
                    Property = DataGridRow.ForegroundProperty,
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
            this.ObjectCard.MinWidth = 410;
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

        private async void UpdateView()
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                DataTable dt = ObjectProcessor.GetObjectsList(this.sourceClass);
                
                if (this.ClassData.ClassType == ClassType.Model)
                {
                    DataView dv = dt.AsDataView();
                    dv.Sort = $"[{ObjectProcessor.NameField}] ASC";
                    this.Dispatcher.Invoke(() =>
                    {
                        this.Data.Columns.Clear();
                        this.Data.ItemsSource = dv;
                    });                   
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.Data.Columns.Clear();
                        this.Data.ItemsSource = dt.AsDataView();
                    });                   
                }
            });          
        }
        private void UpdateViewWithSearch(FieldData data)
        {
            DataTable dt = ObjectProcessor.GetObjectsListWhereLike(this.sourceClass, data.ClassField.VisibleName, data.Value);
            this.Data.Columns.Clear();
            if (this.ClassData.ClassType == ClassType.Model)
            {
                DataView dv = dt.AsDataView();
                dv.Sort = $"[{ObjectProcessor.NameField}] ASC";
                this.Data.ItemsSource = dv;
            }
            else
            {
                this.Data.ItemsSource = dt.AsDataView();
            }
        }
        private void UpdateViewWithFilter(FieldData data)
        {
            DataTable dt = ObjectProcessor.GetObjectsListWhereEqual(this.sourceClass, data.ClassField.VisibleName, data.Value);
            this.Data.Columns.Clear();
            if (this.ClassData.ClassType == ClassType.Model)
            {
                DataView dv = dt.AsDataView();
                dv.Sort = $"[{ObjectProcessor.NameField}] ASC";
                this.Data.ItemsSource = dv;
            }
            else
            {
                this.Data.ItemsSource = dt.AsDataView();
            }
        }
        private void Data_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
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
                    e.Column.MaxWidth = 180;
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

        private void Data_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //int n = int.Parse(((DataTable)e.Row.DataContext)[ObjectProcessor.StatusField].ToString());
            //e.Row.Background = new SolidColorBrush(this.ClassData.Statuses[n].Color);
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            this.OpenNewObject();
        }
        private void OpenNewObject()
        {
            if (this.SourcePreset == null)
            {
                ObjectsEditor oc = new(this.sourceClass, this.SourcePreset);
                oc.OnUpdateRequested += this.ObjectsEditor_OnUpdateRequested;
                oc.Show();
            }
            else
            {
                ObjectsEditor oc = new(this.sourceClass, this.SourcePreset);
                oc.OnUpdateRequested += this.ObjectsEditor_OnUpdateRequested;
                oc.Show();
            }
        }
        private void CopyClick(object sender, RoutedEventArgs e)
        {
            this.OpenCopyOfSelectedObject();
        }

        private void SearchClick(object sender, RoutedEventArgs e)
        {
            this.OpenSearchDialog();
        }
        private void OpenSearchDialog()
        {
            DataSearch ds = new(this.ClassData);
            if (ds.ShowDialog("Поиск", Icon.Search))
            {
                if (ds.OnlyEqual)
                {
                    this.UpdateViewWithFilter(ds.GetData());
                }
                else
                {
                    this.UpdateViewWithSearch(ds.GetData());
                }
            }
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
        private List<Guid> GetSelectedObjectsGuids()
        {
            if (this.Data.SelectedItems.Count == 0)
            {
                return new();
            }
            List<Guid> guids = new();
            foreach (DataRowView obj in this.Data.SelectedItems)
            {
                guids.Add(Guid.Parse(obj.Row[ObjectProcessor.IdField].ToString()));
            }
            return guids;
        }
        private void OpenSelectedObject()
        {
            DialogsManager.ShowWaitCursor(true);
            Guid id = this.GetSelectedObjectGuid();
            if (id == Guid.Empty)
            {
                return;
            }
            Components.Object obj = ObjectProcessor.GetObject(this.sourceClass, id);
            ObjectsEditor oc = new(this.sourceClass, this.SourcePreset, [obj]);
            oc.OnUpdateRequested += this.ObjectsEditor_OnUpdateRequested;
            oc.Show();
        }
        private async void OpenSelectedObjects()
        {
            DialogsManager.ShowWaitCursor(true);
            List<Components.Object> objects = await ObjectProcessor.GetObjects(this.sourceClass, this.GetSelectedObjectsGuids());
            ObjectsEditor oc = new(this.sourceClass, this.SourcePreset, objects);
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
            ObjectsEditor oc = new(this.sourceClass, this.SourcePreset, [obj.Copy()]);
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
                case System.Windows.Input.Key.A:
                case System.Windows.Input.Key.N:
                    this.OpenNewObject();
                    break;
                case System.Windows.Input.Key.Enter:
                    if (this.Data.SelectedItems.Count < 2)
                    {
                        this.OpenSelectedObject();
                    }
                    else
                    {
                        this.OpenSelectedObjects();
                    }
                    break;
                case System.Windows.Input.Key.C:
                case System.Windows.Input.Key.RightShift:
                    this.OpenCopyOfSelectedObject();
                    break;
                case System.Windows.Input.Key.F:
                    this.OpenSearchDialog();
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
                if (this.sourceClass is null)
                {
                    return;
                }
                Guid id = this.GetSelectedObjectGuid();
                if (id == Guid.Empty)
                {
                    this.ObjectCard?.SetEmpty();                
                }
                else
                {
                    Components.Object obj = ObjectProcessor.GetObject(this.sourceClass, id);
                    this.ObjectCard?.UpdateFor(obj);                  
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
