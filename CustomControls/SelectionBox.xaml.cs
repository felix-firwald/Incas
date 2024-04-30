using Common;
using Incubator_2.Windows.CustomDatabase;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incubator_2.CustomControls
{
    /// <summary>
    /// Логика взаимодействия для SelectionBox.xaml
    /// </summary>
    public partial class SelectionBox : UserControl
    {
        public string Value { get { return this.Input.Text; } set { this.Input.Text = value; } }

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
                    ProgramState.ShowErrorDialog($"При попытке определения таблицы возникла ошибка:\n{ex}", "Ошибка");
                }
            }
        }
        public delegate void ValueChanged(object sender, TextChangedEventArgs e);
        public event ValueChanged OnValueChanged;
        public SelectionBox()
        {
            InitializeComponent();
        }

        private void ButtonClick(object sender, MouseButtonEventArgs e)
        {
            DatabaseSelection s = new(this.Database, this.Table, this.Field);
            s.ShowDialog();
            if (s.Result == Windows.DialogStatus.Yes)
            {
                try
                {
                    this.Value = s.SelectedValue;
                }
                catch (Exception ex)
                {
                    ProgramState.ShowErrorDialog(ex.Message);
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
