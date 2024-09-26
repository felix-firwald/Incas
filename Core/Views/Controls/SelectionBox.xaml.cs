using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Incas.CustomDatabases.Views.Windows;
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
        public string Value { get => this.Input.Text; set => this.Input.Text = value; }

        public string Database = "";
        public string Table = "";
        public string Field = "";
        public string Source
        {
            set
            {
                if (value is null)
                {
                    this.Database = "";
                    this.Table = "";
                    this.Field = "";
                }
                else
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
        }
        public delegate void ValueChanged(object sender, TextChangedEventArgs e);
        public event ValueChanged OnValueChanged;
        public SelectionBox()
        {
            this.InitializeComponent();
        }

        private void ButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (this.Database == "")
            {
                DialogsManager.ShowExclamationDialog("Не определена привязка к базе данных!", "Действие прервано");
                return;
            }
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
            Query q = new("")
            {
                typeOfConnection = DBConnectionType.CUSTOM,
                DBPath = ProgramState.GetFullPathOfCustomDb(this.Database)
            };
            q.AddCustomRequest($"SELECT [{this.Field}] FROM [{this.Table}] WHERE [{this.Field}] LIKE '%{this.HintsSearchField.Text}%' LIMIT 4");
            DataTable dt = q.Execute();
            this.HintsList.ItemsSource = dt.AsEnumerable().Select(x => x[0].ToString()).ToList();
            //this.HintsHints
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
