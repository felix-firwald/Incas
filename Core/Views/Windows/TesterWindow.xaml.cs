using Incas.Testing.AutoUI;
using System.Windows;

namespace Incas.Core.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для TesterWindow.xaml
    /// </summary>
    public partial class TesterWindow : Window
    {
        public TesterWindow()
        {
            this.InitializeComponent();
        }

        private void RunServerClick(object sender, RoutedEventArgs e)
        {
            RunServer server = new();
            server.Port = 9000;
            server.ShowDialog("Поднять сервер");
        }

        private void ConnectToServerClick(object sender, RoutedEventArgs e)
        {
            ConnectToServer c = new();
            c.ShowDialog("Подключение к серверу");
        }

        private void MessageToServerClick(object sender, RoutedEventArgs e)
        {
            SendMessageToServer mess = new();
            mess.ShowDialog("Сообщение серверу");
        }

        private void MessageToClientClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
