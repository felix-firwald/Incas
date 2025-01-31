using Incas.Core.Classes;
using Incas.Objects.Engine;
using Incas.Objects.Exceptions;
using Incas.Objects.Views.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Incas.Objects.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ObjectsCorrector.xaml
    /// </summary>
    public partial class ObjectsCorrector : UserControl
    {
        private const string EditedColumn = "edited";
        public Models.Field Field { get; set; }
        public IClass Class { get; set; }
        public FieldFiller Filler { get; set; }
        public ObjectsCorrector(IClass cl, List<Guid> list, Models.Field field)
        {
            this.InitializeComponent();
            this.Field = field;
            this.Class = cl;
            this.ApplyFiller();
            this.ApplyGrid(list);

        }
        public void ApplyFiller()
        {
            this.Filler = new FieldFiller(this.Field);
            this.FillerPanel.Child = this.Filler;
        }
        public void ApplyGrid(List<Guid> list)
        {
            DataTable dt = Processor.GetSimpleObjectsWhereIdForCorrection(this.Class, list, this.Field);
            dt.Columns.Add(EditedColumn, typeof(bool));
            this.Grid.ItemsSource = dt.AsDataView();
        }

        private void ApplyToSelectedClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string value = this.Filler.GetData();
                if (this.Grid.SelectedItems is null || this.Grid.SelectedItems.Count == 0)
                {
                    return;
                }
                foreach (DataRowView dr in this.Grid.SelectedItems)
                {
                    dr.Row[EditedColumn] = true;
                    dr.Row["Значение"] = value;
                }
            }
            catch (NotNullFailed nn)
            {
                DialogsManager.ShowExclamationDialog(nn.Message, "Применение невозможно");
            }            
        }

        private void FinishClick(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> list = new();
            foreach (DataRowView dr in this.Grid.Items)
            {
                bool edited = false;
                bool.TryParse(dr.Row[EditedColumn].ToString(), out edited);
                if (edited)
                {
                    list.Add(dr.Row[Helpers.IdField].ToString(), dr.Row["Значение"].ToString());
                }
                else
                {
                    DialogsManager.ShowExclamationDialog("Не все данные обновлены.", "Сохранение прервано");
                    return;
                }
            }
            this.IsEnabled = false;
            Processor.UpdateFieldsByIdForCorrection(this.Class, list, this.Field);
            DialogsManager.ShowInfoDialog("Исправления успешно применены. Вкладку можно закрыть.");           
        }

        private void Grid_AutoGeneratingColumn(object sender, System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() == Helpers.IdField)
            {
                e.Column.Visibility = Visibility.Hidden;
            }
            else if (e.Column.Header.ToString() == EditedColumn)
            {
                e.Column.Visibility = Visibility.Hidden;
            }
        }
    }
}
