using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Incas.CustomDatabases.Views.Windows;
using Incas.Objects.Components;
using Incas.Objects.Views.Windows;
using System;
using System.Data;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incas.Core.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для SelectionBox.xaml
    /// </summary>
    public partial class SelectionBox : UserControl
    {
        public Objects.Components.Object SelectedObject { get; set; }
        public string Value { get => this.Input.Text; }
        private readonly BindingData Binding;
        public delegate void ValueChanged(object sender, TextChangedEventArgs e);
        public event ValueChanged OnValueChanged;
        public SelectionBox(BindingData bd)
        {
            this.InitializeComponent();
            this.Binding = bd;
        }
        public void SetObject(string guid)
        {
            try
            {
                Guid id = Guid.Parse(guid);
                if (id != Guid.Empty)
                {
                    this.SelectedObject = ObjectProcessor.GetObject(new(this.Binding.Class), id);
                    this.Input.Text = this.SelectedObject.GetFieldValue(this.Binding.Field);
                }
            }
            catch { } 
        }

        private void ButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (this.Binding.Class == Guid.Empty || this.Binding.Field == Guid.Empty)
            {
                DialogsManager.ShowExclamationDialog("Не определена привязка к классу объектов!", "Действие прервано");
                return;
            }
            DatabaseSelection s = new(this.Binding);
            s.ShowDialog();
            if (s.Result == DialogStatus.Yes)
            {
                try
                {
                    this.SelectedObject = s.SelectedObject;
                    this.Input.Text = this.SelectedObject.GetFieldValue(this.Binding.Field);
                }
                catch (Exception ex)
                {
                    DialogsManager.ShowErrorDialog(ex.Message);
                }
            }
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnValueChanged?.Invoke(this, e);
        }

        private void FindClick(object sender, MouseButtonEventArgs e)
        {
            this.Hints.IsOpen = true;
            this.HintsSearchField.Focusable = true;
            this.HintsSearchField.Focus();
        }

        private void CancelSearch(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Hints.IsOpen = false;
        }

        private void HintsSearchField_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Query q = new("")
            //{
            //    typeOfConnection = DBConnectionType.CUSTOM,
            //    DBPath = ProgramState.GetFullPathOfCustomDb(this.Database)
            //};
            //q.AddCustomRequest($"SELECT [{this.Field}] FROM [{this.Table}] WHERE [{this.Field}] LIKE '%{this.HintsSearchField.Text}%' LIMIT 4");
            //DataTable dt = q.Execute();
            //this.HintsList.ItemsSource = dt.AsEnumerable().Select(x => x[0].ToString()).ToList();
            ////this.HintsHints
        }

        private void HintsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedValue is not null)
            {
                this.Input.Text = ((ListBox)sender).SelectedValue.ToString();
                this.Hints.IsOpen = false;
            }          
        }
    }
}
