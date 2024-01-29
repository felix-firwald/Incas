using Common;
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
using System.Windows.Shapes;

namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для DialogInput.xaml
    /// </summary>
    public partial class DialogInput : Window
    {
        public string Input { get { return this.InputValue.Text; } }
        public DialogInput(string title, string description = "Введите значение")
        {
            InitializeComponent();
            this.Title.Content = title;
            this.InputValue.Tag = description;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Input))
            {
                ProgramState.ShowExclamationDialog("Поле пустое!", "Действие невозможно");
                return;
            }
            this.Close();
        }

        private void InputValue_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
