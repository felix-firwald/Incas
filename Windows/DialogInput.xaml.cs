using Common;
using System.Windows;
using System.Windows.Controls;

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
            this.TitleText.Content = title;
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
