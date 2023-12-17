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
    public enum DialogStatus
    {
        Yes,
        No,
        Undefined
    }
    /// <summary>
    /// Логика взаимодействия для DialogQuestion.xaml
    /// </summary>
    public partial class DialogQuestion : Window
    {
        public DialogStatus status = DialogStatus.Undefined;
        public DialogQuestion(string text, string title = "Выполнить действие?", string yesText = "Да", string noText = "Нет")
        {
            InitializeComponent();
            this.Title.Content = title;
            this.Description.Text = text;
            this.Yes.Content = yesText;
            this.No.Content = noText;
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            status = DialogStatus.Yes;
            this.Close();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            status = DialogStatus.No;
            this.Close();
        }
    }
}
