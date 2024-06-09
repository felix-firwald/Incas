using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Incas.CustomDatabases.Views.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incas.Core.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для SelectionBox.xaml
    /// </summary>
    public partial class SelectionBox : UserControl
    {
        public string Value { get => this.Input.Text; set => this.Input.Text = value; }

        public string Database = "";
        public string Table = "";
        public string Field = "";
        public string Source
        {
            set
            {
                try
                {
                    this.Database = value.Split('.')[0];
                    this.Table = value.Split('.')[1];
                    this.Field = value.Split('.')[2];
                }
                catch (Exception ex)
                {
                    DialogsManager.ShowErrorDialog($"При попытке определения таблицы возникла ошибка:\n{ex}", "Ошибка");
                }
            }
        }
        public delegate void ValueChanged(object sender, TextChangedEventArgs e);
        public event ValueChanged OnValueChanged;
        public SelectionBox()
        {
            this.InitializeComponent();
        }

        private void ButtonClick(object sender, MouseButtonEventArgs e)
        {
            DatabaseSelection s = new(this.Database, this.Table, this.Field);
            s.ShowDialog();
            if (s.Result == DialogStatus.Yes)
            {
                try
                {
                    this.Value = s.SelectedValue;
                }
                catch (Exception ex)
                {
                    DialogsManager.ShowErrorDialog(ex.Message);
                    this.Value = "";
                }
            }
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnValueChanged?.Invoke(this, e);
        }
    }
}
