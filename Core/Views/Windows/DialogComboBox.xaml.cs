using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Incas.Core.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для DialogComboBox.xaml
    /// </summary>
    public partial class DialogComboBox : Window
    {
        public string SelectedValue => this.InputValue.Text;
        public DialogComboBox(List<string> elements, string title, string description)
        {
            this.InitializeComponent();
            this.TitleText.Content = title;
            this.Description.Text = description;
            this.LoadElements(elements);
        }
        private void LoadElements(List<string> elements)
        {
            foreach (string element in elements)
            {
                this.InputValue.Items.Add(element);
            }
            if (this.InputValue.Items.Count > 0)
            {
                this.InputValue.SelectedIndex = 0;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
