using Incas.Core.Classes;
using System.Windows;
using System.Windows.Controls;

namespace Incas.Core.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для DialogInput.xaml
    /// </summary>
    public partial class DialogInput : Window
    {
        public string Input => this.InputValue.Text;
        public DialogInput(string title, string description = "Введите значение")
        {
            this.InitializeComponent();
            this.TitleText.Content = title;
            this.InputValue.Tag = description;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.Input))
            {
                DialogsManager.ShowExclamationDialog("Поле пустое!", "Действие невозможно");
                return;
            }
            this.Close();
        }

        private void InputValue_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
