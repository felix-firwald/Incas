using System.Windows;

namespace Incas.Core.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для DialogQuestion.xaml
    /// </summary>
    public partial class DialogQuestion : Window
    {
        public DialogStatus status = DialogStatus.Undefined;
        public DialogQuestion(string text, string title = "Выполнить действие?", string yesText = "Да", string noText = "Нет")
        {
            this.InitializeComponent();
            this.TitleText.Content = title;
            this.Description.Text = text;
            this.Yes.Content = yesText;
            this.No.Content = noText;
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            this.status = DialogStatus.Yes;
            this.Close();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            this.status = DialogStatus.No;
            this.Close();
        }
    }
}
