using System.Windows;

namespace Incas.Core.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для Error.xaml
    /// </summary>
    public partial class Dialog : Window
    {
        public enum DialogIcon
        {
            Error,
            DatabaseError,
            AccessDenied,
            Exclamation,
            Info
        }
        public Dialog(string text, string title = "Неизвестная ошибка", DialogIcon ic = DialogIcon.Error)
        {
            this.InitializeComponent();
            this.TitleText.Content = title;
            this.Description.Text = text;
            switch (ic)
            {
                case DialogIcon.Error:
                    this.IconError.Visibility = Visibility.Visible;
                    break;
                case DialogIcon.DatabaseError:
                    this.IconDatabaseError.Visibility = Visibility.Visible;
                    break;
                case DialogIcon.AccessDenied:
                    this.IconAccessDenied.Visibility = Visibility.Visible;
                    break;
                case DialogIcon.Exclamation:
                    this.IconExclamation.Visibility = Visibility.Visible;
                    break;
                case DialogIcon.Info:
                    this.IconInfo.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
