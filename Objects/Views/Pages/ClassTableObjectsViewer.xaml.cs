using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Table = IncasEngine.ObjectiveEngine.Models.Table;
using System.Data;
using IncasEngine.Core.DatabaseQueries.RequestsUtils.Where;
using Incas.DialogSimpleForm.Components;
using Incas.Core.Converters;
using Incas.Core.Classes;

namespace Incas.Objects.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ClassTableObjectsViewer.xaml
    /// </summary>
    public partial class ClassTableObjectsViewer : UserControl
    {
        public IClass Class { get; set; }
        public IClassData ClassData { get; set; }
        public Table Table { get; set; }
        private WhereInstruction where { get; set; }
        private DataView View { get; set; }
        public ClassTableObjectsViewer(IClass cl, Table table)
        {
            this.InitializeComponent();
            this.Class = cl;
            this.ClassData = this.Class.GetClassData();
            this.Table = table;
            this.UpdateView();
        }

        private void UpdateView()
        {
            DataTable dt = Processor.GetSummaryListBasic(this.Class, this.Table, this.where);
            this.Dispatcher.Invoke(() =>
            {
                this.Data.Columns.Clear();
                this.Data.ItemsSource = dt.AsDataView();
            });
        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            this.CancelSearchButton.Visibility = Visibility.Collapsed;
            this.UpdateView();
        }

        private void SearchClick(object sender, RoutedEventArgs e)
        {

        }

        private void CancelSearchClick(object sender, RoutedEventArgs e)
        {

        }

        private void ExportClick(object sender, RoutedEventArgs e)
        {

        }

        private void OpenInAnotherWindowClick(object sender, RoutedEventArgs e)
        {

        }

        private void Data_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.Column.Header.ToString())
            {
                case Helpers.IdField:
                    e.Column.Visibility = Visibility.Hidden;
                    break;
                case Helpers.StatusField:
                    e.Column.Visibility = Visibility.Hidden;
                    break;
                case Helpers.TargetClassField:
                case Helpers.TargetObjectField:
                    e.Column.Visibility = Visibility.Hidden;
                    break;
                default:
                    string colHeader = e.Column.Header.ToString();
                    foreach (Field f in this.Table.Fields)
                    {
                        if (f.Type is FieldType.Boolean && colHeader == f.VisibleName)
                        {
                            System.Windows.Data.Binding binding = new(colHeader);
                            binding.Converter = new StringBooleanToBooleanConverter();
                            DataGridCheckBoxColumn cbc = new()
                            {
                                Header = f.VisibleName,
                                Binding = binding,
                                EditingElementStyle = ResourceStyleManager.FindStyle(ResourceStyleManager.CheckboxEditingGridStyle),
                                ElementStyle = ResourceStyleManager.FindStyle(ResourceStyleManager.CheckboxNotEditableGridStyle)
                            };
                            e.Column = cbc;
                            break;
                        }
                    }
                    break;
            }
        }

        private void Data_LoadingRow(object sender, DataGridRowEventArgs e)
        {

        }

        private void Data_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Data_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }
        private Guid GetSelectedObjectGuid()
        {
            if (this.Data.SelectedItems.Count == 0)
            {
                return Guid.Empty;
            }
            string source = ((DataRowView)this.Data.SelectedItems[0]).Row[Helpers.TargetObjectField].ToString();
            return Guid.Parse(source);
        }
        private void OnMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                Guid id = this.GetSelectedObjectGuid();
                if (id != Guid.Empty)
                {
                    DialogsManager.ShowEditor(this.Class, Processor.GetObject(this.Class, id));
                }
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }
        }
    }
}
