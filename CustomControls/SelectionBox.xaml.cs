using Common;
using ICSharpCode.AvalonEdit.Document;
using Incubator_2.Windows.CustomDatabase;
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
                    Database = value.Split('.')[0];
                    Table = value.Split('.')[1];
                    Field = value.Split('.')[2];
                }
                catch(Exception ex)
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
            DatabaseSelection s = new(Database, Table, Field);
            s.ShowDialog();
            if (s.Result == Windows.DialogStatus.Yes)
            {
                try
                {
                    this.Value = s.SelectedValue;
                }
                catch(Exception ex)
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
