using Incas.Core.Classes;
using System.Windows;
using System.Windows.Input;

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
            this.Title = "";
            this.TitleText.Content = title;
            this.Description.Text = text;
            this.Yes.Content = yesText;
            this.No.Content = noText;
            DialogsManager.ShowWaitCursor(false);
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

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
