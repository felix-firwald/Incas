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

namespace Incas.Core.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для DialogComboBox.xaml
    /// </summary>
    public partial class DialogComboBox : Window
    {
        public string SelectedValue
        {
            get
            {
                return this.InputValue.Text;
            }
        }
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
