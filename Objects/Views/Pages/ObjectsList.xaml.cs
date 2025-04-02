using DocumentFormat.OpenXml.Bibliography;
using Incas.Core.Classes;
using Incas.Core.Interfaces;
using Incas.Core.Views.Controls;
using Incas.Objects.AutoUI;
using Incas.Objects.Views.Controls;
using Incas.Objects.Views.Windows;
using Incas.Rendering.AutoUI;
using Incas.Rendering.Components;
using IncasEngine.Core;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Exceptions;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Groups.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using static Incas.Core.Interfaces.ITabItem;

namespace Incas.Objects.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ObjectsList.xaml
    /// </summary>
    public partial class ObjectsList : UserControl, ITabItem
    {
        public enum OpenType
        {
            InPage,
            InWindow
        }
        private const string ColorColumn = "__COLOR_COLUMN__";
        public Style ColumnHeaderSpecialStyle { get; set; }
        public IClass sourceClass;
        public IClassData ClassData;
        private ObjectCard ObjectCard;
        private GroupClassPermissionSettings permissionSettings;
        public event TabAction OnClose;
        public string Id { get; set; }
        public OpenType ShowTypeButtonAction { get; set; }
        private DataView View { get; set; }

        private ObjectsListLoading loading;
        public delegate void ObjectsListAction(IClass source);
        public event ObjectsListAction OnPresetsViewRequested;

        public ObjectsList(IClass source)
        {
            this.InitializeComponent();
            this.ColumnHeaderSpecialStyle = this.FindResource("ColumnHeaderSpecial") as Style;
                      
            DialogsManager.ShowWaitCursor();
            this.ApplyClass(source);
            this.ApplyGroupConstraints();
            DialogsManager.ShowWaitCursor(false);          
            source.OnUpdated += this.EngineEvents_OnUpdateClassRequested;
        }

        private void ApplyClass(IClass source)
        {
            this.sourceClass = source;
            this.permissionSettings = this.GetPermissionSettings();
            this.ClassData = source.GetClassData();           
            if (this.ClassData.ShowCard)
            {
                this.PlaceCard();
            }
            this.UpdateView();
        }

        private void EngineEvents_OnUpdateClassRequested()
        {
            ClassUpdatedMessage message = new();
            this.MainGrid.Children.Clear();
            this.MainGrid.Children.Add(message);
            Grid.SetRow(message, 1);
            Grid.SetRowSpan(message, 2);
            Grid.SetColumn(message, 0);
        }
        
        private GroupClassPermissionSettings GetPermissionSettings()
        {
            return ProgramState.CurrentWorkspace.CurrentGroup.GetClassPermissions(this.sourceClass.Id);
        }
        private void PlaceLoadingControl()
        {
            ObjectsListLoading loading = new();
            this.loading = loading;
            this.MainGrid.Children.Add(loading);
            Grid.SetRow(loading, 1);
            Grid.SetRowSpan(loading, 2);
            Grid.SetColumn(loading, 0);
        }
        private void HideLoadingControl()
        {
            this.MainGrid.Children.Remove(this.loading);
        }
        private void ApplyGroupConstraints()
        {
            if (!this.permissionSettings.ViewOperations)
            {
                this.Tools.Visibility = Visibility.Collapsed;
                this.Data.Visibility = Visibility.Collapsed;
                NoPermission np = new();
                this.MainGrid.Children.Add(np);
                Grid.SetRow(np, 1);
                Grid.SetRowSpan(np, 2);
                Grid.SetColumn(np, 0);
                return;
            }
            if (!this.permissionSettings.CreateOperations)
            {
                this.AddButton.Visibility = Visibility.Collapsed;
                this.CopyButton.Visibility = Visibility.Collapsed;
                this.ParseButton.Visibility = Visibility.Collapsed;
            }
            if (!this.permissionSettings.DeleteOperations)
            {
                this.RemoveButton.Visibility = Visibility.Collapsed;
            }            
        }
        public void TrySelectObject(Guid objectId)
        {
            string objectIdString = objectId.ToString();
            for (int i = 0; i < this.View.Count; i++)
            {
                DataRowView row = this.View[i];
                if (row[0].ToString() == objectIdString)
                {
                    this.Data.SelectedIndex = i;
                    this.Data.ScrollIntoView(row);
                    break;
                }
            }
        }
        private void PlaceCard()
        {
            if (this.permissionSettings.ViewOperations)
            {
                this.ObjectCard = new(this.sourceClass);
                this.ObjectCard.OnFilterRequested += this.ObjectCard_OnFilterRequested;
                this.ObjectCard.MinWidth = 410;
                this.MainGrid.Children.Add(this.ObjectCard);
                Grid.SetRow(this.ObjectCard, 1);
                Grid.SetRowSpan(this.ObjectCard, 2);
                Grid.SetColumn(this.ObjectCard, 1);
                this.ObjectCard.SetEmpty();
            }
        }

        private void ObjectCard_OnFilterRequested(IncasEngine.ObjectiveEngine.Common.FieldData data)
        {
            this.UpdateViewWithFilter(data);
        }

        private async void UpdateView()
        {
            DataTable dt = await Processor.GetObjectsList(this.sourceClass);                
            if (this.sourceClass.Type == ClassType.Model)
            {
                this.View = dt.AsDataView();
                this.View.Sort = $"[{Helpers.NameField}] ASC";
                this.Dispatcher.Invoke(() =>
                {
                    this.Data.Columns.Clear();
                    this.Data.ItemsSource = this.View;
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
        }
        public async void UpdateViewWithSearch(FieldData data)
        {
            DataTable dt = await Processor.GetObjectsListWhereLike(this.sourceClass, data.ClassField.VisibleName, data.Value);
            this.Data.Columns.Clear();
            this.CancelSearchButton.Visibility = Visibility.Visible;
            if (this.sourceClass.Type == ClassType.Model)
            {
                DataView dv = dt.AsDataView();
                dv.Sort = $"[{Helpers.NameField}] ASC";
                this.Data.ItemsSource = dv;
            }
            else
            {
                this.Data.ItemsSource = dt.AsDataView();
            }
        }
        public async void UpdateViewWithFilter(FieldData data)
        {
            DataTable dt = await Processor.GetObjectsListWhereEqual(this.sourceClass, data.ClassField.VisibleName, data.Value);
            this.Data.Columns.Clear();
            this.CancelSearchButton.Visibility = Visibility.Visible;
            if (this.sourceClass.Type == ClassType.Model)
            {
                DataView dv = dt.AsDataView();
                dv.Sort = $"[{Helpers.NameField}] ASC";
                this.Data.ItemsSource = dv;
            }
            else
            {
                this.Data.ItemsSource = dt.AsDataView();
            }
        }
        private void Data_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.Column.Header.ToString())
            {
                case Helpers.IdField:
                    e.Column.Visibility = Visibility.Hidden;
                    break;
                case Helpers.NameField:
                    if (this.ClassData.ShowCard)
                    {
                        e.Column.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        e.Column.Header = "Наименование";
                        e.Column.HeaderStyle = this.ColumnHeaderSpecialStyle;
                        e.Column.MinWidth = 40;
                        e.Column.MaxWidth = 200;
                    }                    
                    break;
                case Helpers.DateCreatedField:
                    e.Column.Header = "Дата создания";
                    e.Column.HeaderStyle = this.ColumnHeaderSpecialStyle;
                    e.Column.MinWidth = 40;
                    break;
                case Helpers.StatusField:
                    e.Column.Visibility = Visibility.Hidden;
                    break;
                case Helpers.TargetClassField:
                case Helpers.TargetObjectField:
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
            if (!this.permissionSettings.CreateOperations)
            {
                DialogsManager.ShowAccessErrorDialog("Вы не вправе создавать объекты этого класса.");
                return;
            }
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
            this.OpenSearchDialog();
        }
        private void OpenSearchDialog()
        {
            DataSearch ds = new(this.ClassData as ClassDataBase);
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
            this.CancelSearchButton.Visibility = Visibility.Collapsed;
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
            string source = ((DataRowView)this.Data.SelectedItems[0]).Row[Helpers.IdField].ToString();
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
                guids.Add(Guid.Parse(obj.Row[Helpers.IdField].ToString()));
            }
            return guids;
        }
        private void OpenSelectedObject()
        {
            if (!this.permissionSettings.ReadOperations)
            {
                DialogsManager.ShowAccessErrorDialog("Вы не можете просматривать объекты этого класса.");
                return;
            }
            try
            {
                DialogsManager.ShowWaitCursor(true);
                Guid id = this.GetSelectedObjectGuid();
                if (id == Guid.Empty)
                {
                    return;
                }
                IObject obj = Processor.GetObject(this.sourceClass, id);
                if (obj is IHierarchical objHierarchical && objHierarchical.Child != Guid.Empty)
                {
                    ObjectsEditor oc = new(EngineGlobals.GetClass(objHierarchical.Child), [obj]);
                    oc.OnUpdateRequested += this.ObjectsEditor_OnUpdateRequested;
                    oc.Show();
                }
                else
                {
                    ObjectsEditor oc = new(this.sourceClass, [obj]);
                    oc.OnUpdateRequested += this.ObjectsEditor_OnUpdateRequested;
                    oc.Show();
                }
                
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }
        }
        private async void OpenSelectedObjects()
        {
            if (!this.permissionSettings.ReadOperations)
            {
                DialogsManager.ShowAccessErrorDialog("Вы не вправе просматривать объекты этого класса.");
                return;
            }
            DialogsManager.ShowWaitCursor(true);
            try
            {
                List<IObject> objects = await Processor.GetObjects(this.sourceClass, this.GetSelectedObjectsGuids());
                ObjectsEditor oc = new(this.sourceClass, objects);
                oc.OnUpdateRequested += this.ObjectsEditor_OnUpdateRequested;
                oc.Show();
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }
        }

        private void OpenCopyOfSelectedObject()
        {
            if (!this.permissionSettings.CreateOperations)
            {
                DialogsManager.ShowAccessErrorDialog("Вы не вправе создавать объекты этого класса.");
                return;
            }
            Guid id = this.GetSelectedObjectGuid();
            if (id == Guid.Empty)
            {
                return;
            }
            IObject obj = Processor.GetObject(this.sourceClass, id);
            List<IObject> objects = new();
            objects.Add(obj.Copy());
            ObjectsEditor oc = new(this.sourceClass, objects);
            oc.OnUpdateRequested += this.ObjectsEditor_OnUpdateRequested;
            oc.Show();
        }
        private void ObjectsEditor_OnUpdateRequested()
        {
            this.UpdateView();
        }
        private void RemoveSelectedObject()
        {
            if (!this.permissionSettings.DeleteOperations)
            {
                DialogsManager.ShowAccessErrorDialog("Вы не вправе удалять объекты этого класса.");
                return;
            }
            Guid id = this.GetSelectedObjectGuid();
            if (id == Guid.Empty)
            {
                return;
            }
            IObject obj = Processor.GetObject(this.sourceClass, id);
            if (this.ClassData.EditByAuthorOnly == true && !Helpers.CheckAuthor(obj))
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
                try
                {
                    obj.Remove();
                }
                catch (AccessException ex)
                {
                    DialogsManager.ShowAccessErrorDialog(ex);
                }
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
                if (id == Guid.Empty || this.Data.SelectedItems.Count > 1)
                {
                    this.ObjectCard?.SetEmpty();                
                }
                else
                {
                    IObject obj = Processor.GetObject(this.sourceClass, id);
                    this.ObjectCard?.UpdateFor(obj);              
                }
            }        
        }

        private void OpenInAnotherWindowClick(object sender, RoutedEventArgs e)
        {
            if (this.sourceClass == null)
            {
                return;
            }
            ObjectsList ol = new(this.sourceClass);
            ol.ShowTypeButtonAction = OpenType.InWindow;
            switch (this.ShowTypeButtonAction)
            {
                case OpenType.InPage:
                    GroupBox gb = new()
                    {
                        Header = this.ClassData.ListName,
                        Content = ol
                    };
                    DialogsManager.ShowPage(gb, this.ClassData.ListName, MainWindowButtonTab.ClassPrefix + this.sourceClass.Id.ToString());
                    break;
                case OpenType.InWindow:
                    ol.ShowTypeButtonAction = OpenType.InPage;
                    DialogsManager.ShowWindow(this.ClassData.ListName, ol);
                    break;
            }            
        }

        private void OpenPresetsListClick(object sender, RoutedEventArgs e)
        {
            this.OnPresetsViewRequested?.Invoke(this.sourceClass);
        }

        private void CMOpenObjectClick(object sender, RoutedEventArgs e)
        {
            this.OpenSelectedObject();
        }

        private void AggregateClick(object sender, RoutedEventArgs e)
        {
            DialogsManager.ShowInfoDialog("Функционал не реализован");
        }

        private void CompareClick(object sender, RoutedEventArgs e)
        {
            DialogsManager.ShowInfoDialog("Функционал не реализован");
        }

        private void ShowHistoryClick(object sender, RoutedEventArgs e)
        {
            Guid id = this.GetSelectedObjectGuid();
            if (id == Guid.Empty)
            {
                DialogsManager.ShowExclamationDialog("Не выбран объект для просмотра версий.", "Действие невозможно");
                return;
            }
            ObjectEditions editions = new(this.sourceClass, Processor.GetObject(this.sourceClass, id));
            DialogsManager.ShowPageWithGroupBox(editions, "История версий", id.ToString());
        }

        private void ExportClick(object sender, RoutedEventArgs e)
        {
            DialogsManager.ShowInfoDialog("Функционал не реализован");
        }

        private void ParseClick(object sender, RoutedEventArgs e)
        {
            ParseSettings parse = new();
            if (parse.ShowDialog("Настройки парсинга"))
            {
                IObject obj = ObjectParser.ParseString(parse.Source, parse.Pattern, this.sourceClass);
                ObjectsEditor editor = new(this.sourceClass, [obj]);
                editor.Show();
            }
        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            this.CancelSearchButton.Visibility = Visibility.Collapsed;
            this.UpdateView();
        }

        private void ConvertClick(object sender, RoutedEventArgs e)
        {
            DialogsManager.ShowInfoDialog("Функционал не реализован");
        }

        private void ShowBackReferencesClick(object sender, RoutedEventArgs e)
        {
            Guid id = this.GetSelectedObjectGuid();
            if (id == Guid.Empty)
            {
                DialogsManager.ShowExclamationDialog("Не выбран объект для просмотра обратных ссылок.", "Действие невозможно");
                return;
            }
            ObjectBackReferences backReferenceViewer = new(this.sourceClass, Processor.GetObject(this.sourceClass, id));
            DialogsManager.ShowPageWithGroupBox(backReferenceViewer, "Список обратных ссылок", id.ToString());
        }

        private async void ExportXMLClick(object sender, RoutedEventArgs e)
        {
            string path = "";
            List<Guid> ids = this.GetSelectedObjectsGuids();
            if (ids.Count == 0)
            {
                DialogsManager.ShowExclamationDialog("Не выбраны объекты для экспорта", "Действие невозможно");
                return;
            }            
            if (DialogsManager.ShowSaveFileDialog(ref path, ".xml|XML"))
            {
                List<IObject> objects = await Processor.GetObjects(this.sourceClass, ids);
                Processor.ConvertObjectsToXml(objects, path);
            }
        }

        private async void ExportJSONClick(object sender, RoutedEventArgs e)
        {
            string path = "";
            List<Guid> ids = this.GetSelectedObjectsGuids();
            if (ids.Count == 0)
            {
                DialogsManager.ShowExclamationDialog("Не выбраны объекты для экспорта", "Действие невозможно");
                return;
            }
            if (DialogsManager.ShowSaveFileDialog(ref path, ".json|JSON"))
            {
                List<IObject> objects = await Processor.GetObjects(this.sourceClass, ids);
                Processor.ConvertObjectsToJson(objects, path);
            }
        }

        private void JumpToChildClick(object sender, RoutedEventArgs e)
        {
            Guid id = this.GetSelectedObjectGuid();
            if (id == Guid.Empty)
            {
                DialogsManager.ShowExclamationDialog("Не выбран объект для просмотра наследника.", "Действие невозможно");
                return;
            }

            IObject target = Processor.GetObject(this.sourceClass, id);
            if (target is IHierarchical objHierarchical && objHierarchical.Child != Guid.Empty)
            {
                IClass targetClass = EngineGlobals.GetClass(objHierarchical.Child);
                ObjectsList oc = new(targetClass);
                DialogsManager.ShowPageWithGroupBox(oc, "Поиск наследника", target.Id.ToString());
                oc.TrySelectObject(target.Id);
            }
        }
    }
}
