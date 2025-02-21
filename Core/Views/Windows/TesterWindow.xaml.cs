using Incas.Testing.AutoUI;
using IncasEngine.ClientServer.Core;
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
