using Incas.Core.Classes;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Incas.Server.Components
{
    internal static class Client
    {
        internal static IPAddress IpAdress;
        internal static int Port = ProgramState.DefaultPort;
        //internal static TcpClient Me;
        private static NetworkStream stream;
        internal async static void ConnectToService()
        {
            IPEndPoint ipEndPoint = new(IpAdress, Port);
            TcpClient client = new();
            await client.ConnectAsync(ipEndPoint);
            stream = client.GetStream();           
        }
        internal static async void Write(ClientMessage message)
        {
            // считываем строку в массив байтов
            // при отправке добавляем маркер завершения сообщения - \n
            byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message) + CommonClientServerLogic.EndMessage);
            // отправляем данные
            if (stream == null)
            {
                DialogsManager.ShowErrorDialog("Произошел разрыв соединения. Будет выполнено переподключение.");
                ConnectToService();
            }
            await stream.WriteAsync(data);
        }
    }
}
