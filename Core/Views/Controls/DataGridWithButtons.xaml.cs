using Incas.Core.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.Data;

namespace Incas.Core.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для DataGridWithButtons.xaml
    /// </summary>

    internal class DGWBViewModel : BaseViewModel
    {
        private DataTable _data;
        private int selected;
        public DGWBViewModel(DataTable t)
        {
            this._data = t;
        }
        public DataTable Grid
        {
            get => this._data;
            set
            {
                this._data = value;
                this.OnPropertyChanged(nameof(this.Grid));
            }
        }
        public int SelectedIndex
        {
            get => this.selected;
            set
            {
                this.selected = value;
                this.OnPropertyChanged(nameof(this.SelectedIndex));
            }
        }
    }

    public partial class DataGridWithButtons : UserControl
    {
        public DataTable DataTable => this.vm.Grid;
        private DGWBViewModel vm;
        public DataGridWithButtons(DataTable table)
        {
            this.InitializeComponent();
            this.vm = new(table);
            this.DataContext = this.vm;
        }

        private void AddRowClick(object sender, MouseButtonEventArgs e)
        {
            this.vm.Grid.Rows.Add();
        }

        private void RemoveRowClick(object sender, MouseButtonEventArgs e)
        {
            if (this.vm.SelectedIndex > 0)
            {
                this.vm.Grid.Rows[this.vm.SelectedIndex].Delete();
            }
        }
        private void RowUpClick(object sender, MouseButtonEventArgs e)
        {

        }
        private void RowDownClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void Grid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            foreach (DataColumn dc in this.vm.Grid.Columns)
            {
                if (dc.ColumnName == e.Column.Header.ToString())
                {
                    DataGridTextColumn dgt1 = new();
                    dgt1.Header = dc.ColumnName;
                    dgt1.Binding = new System.Windows.Data.Binding(dc.ColumnName);
                    dgt1.EditingElementStyle = this.FindResource("TextBoxGrid") as Style;
                    e.Column = dgt1;
                }
            }          
        }
    }
}
