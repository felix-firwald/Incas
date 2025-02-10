using DocumentFormat.OpenXml.Office2010.Excel;
using Incas.Objects.ViewModels;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Interfaces;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Incas.Objects.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ObjectEditions.xaml
    /// </summary>
    public partial class ObjectEditions : UserControl
    {
        public ObjectEditionsViewModel vm { get; set; }
        public Style ColumnHeaderSpecialStyle { get; set; }
        public ObjectEditions(IClass @class, IObject @object)
        {
            this.InitializeComponent();
            this.ColumnHeaderSpecialStyle = this.FindResource("ColumnHeaderSpecial") as Style;
            this.vm = new(@class, @object);
            this.vm.OnSelectedVersionChanged += this.Vm_OnSelectedVersionChanged;
            this.SelectedVersionTab.IsEnabled = false;
            this.DataContext = this.vm;
            this.PlaceActualCard();
            this.LoadData();
        }

        private void Vm_OnSelectedVersionChanged(Guid version)
        {
            IObjectEdition edit = Processor.GetEdition(this.vm.TargetClass, this.vm.TargetObject, version);
            this.PlaceVersionCard(edit);
            this.SelectedVersionTab.IsEnabled = true;
            this.SelectedVersionTab.IsSelected = true;
            this.PlaceVersionCard(edit);
        }

        public void PlaceActualCard()
        {
            ObjectCard card = new(this.vm.TargetClass, false);
            card.UpdateFor(this.vm.TargetObject);
            this.ActualCardPlacer.Child = card;
        }
        public void PlaceVersionCard(IObjectEdition edit)
        {
            ObjectCard card = new(this.vm.TargetClass, false);
            card.UpdateFor(edit);
            this.SelectedVersionCardPlacer.Child = card;
        }
        public void LoadData()
        {
            DataTable dt = Processor.GetEditsList(this.vm.TargetClass, this.vm.TargetObject.Id);
            this.Data.Columns.Clear();
            this.Data.ItemsSource = dt.AsDataView();
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

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            this.SelectedVersionTab.IsEnabled = false;
            this.ActualVersionTab.IsSelected = true;
            this.vm.SelectedField = null;
            this.PlaceActualCard();
            this.LoadData();
        }

        private void SearchClick(object sender, RoutedEventArgs e)
        {

        }

        private void CancelSearchClick(object sender, RoutedEventArgs e)
        {

        }

        private void CompareClick(object sender, RoutedEventArgs e)
        {

        }

        private void RollbackClick(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveSelectedVersionClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {          
            switch (e.Column.Header.ToString())
            {
                case Helpers.IdField:
                    e.Column.Visibility = Visibility.Hidden;
                    break;
                case Helpers.DateCreatedField:
                    e.Column.Header = "Дата версии";
                    e.Column.HeaderStyle = this.ColumnHeaderSpecialStyle;
                    e.Column.MinWidth = 100;
                    e.Column.MaxWidth = 140;
                    e.Column.CanUserReorder = false;
                    break;
                case Helpers.AuthorField:
                    e.Column.Header = "Редактор";
                    e.Column.MinWidth = 100;
                    e.Column.HeaderStyle = this.ColumnHeaderSpecialStyle;
                    break;
            }
        }

        private void Data_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Guid id = this.GetSelectedObjectGuid();
            if (id == Guid.Empty)
            {
                this.ActualVersionTab.IsSelected = true;
                this.SelectedVersionTab.IsEnabled = false;
                return;
            }
            IObjectEdition edit = Processor.GetEdition(this.vm.TargetClass, this.vm.TargetObject, id);
            this.PlaceVersionCard(edit);
            this.SelectedVersionTab.IsEnabled = true;
            this.SelectedVersionTab.IsSelected = true;
        }
    }
}
