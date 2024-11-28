using Incas.Core.Classes;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Newtonsoft.Json;
using Incas.Server.Interfaces;

namespace Incas.Server.Components
{
    internal static class Server
    {
        internal static IPAddress IpAdress
        {
            get
            {
                return ProgramState.GetGlobalIPAddress();
            }
        }
        internal static int Port = ProgramState.DefaultPort;
        private static bool isRunning = false;
        internal static bool IsRunning
        {
            get
            {
                return isRunning;
            }
            set
            {
                isRunning = value;
                InitializeServer();
            }
        }
        internal static TcpListener Listener;
        private async static void InitializeServer()
        {
            try
            {
                Listener = new(IPAddress.Any, Port);
                Listener.Start();
            }
            catch (System.Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }
            while (IsRunning)
            {
                TcpClient tcpClient = await Listener.AcceptTcpClientAsync();
                await Task.Run(() => ProcessClientAsync(tcpClient));
            }
            Listener.Stop();
        }
        private static async Task ProcessClientAsync(TcpClient tcpClient)
        {
            await Task.Run(() =>
            {
                NetworkStream stream = tcpClient.GetStream();
                // буфер для хуйни
                List<byte> response = new();
                int bytesRead = 10;
                while (IsRunning)
                {
                    try
                    {
                        ProgramStatusBar.SetText("найден байт");
                        while ((bytesRead = stream.ReadByte()) != CommonClientServerLogic.EndMessage && bytesRead != -1)
                        {                            
                            response.Add((byte)bytesRead);
                        }
                        response.RemoveAt(response.Count - 1);
                        string text = Encoding.UTF8.GetString(response.ToArray());
                        ClientMessage cm = JsonConvert.DeserializeObject<ClientMessage>(text);
                        switch (cm.Direction)
                        {
                            case DirectionMode.Request:
                                switch (cm.Type)
                                {
                                    case ClientMessageType.InitialRequest:
                                        DialogsManager.ShowInfoDialog("Направлен пакет InitialRequest");                                       
                                        break;
                                    case ClientMessageType.Authorization:
                                        DialogsManager.ShowInfoDialog("Направлен пакет Authorization");
                                        break;
                                    case ClientMessageType.ServiceAction:
                                        DialogsManager.ShowInfoDialog("Направлен пакет ServiceAction");
                                        break;
                                    case ClientMessageType.ObjectAction:
                                        DialogsManager.ShowInfoDialog("Направлен пакет ObjectAction");
                                        break;
                                }
                                break;
                            case DirectionMode.Response:
                                break;
                        }
                        ProgramStatusBar.Hide();
                    }
                    catch (Exception ex)
                    {
                        DialogsManager.ShowErrorDialog(ex, "При анализе TCP/IP пакета возникла ошибка");
                        tcpClient.Close();
                        return;
                    }
                    response.Clear();
                }
                tcpClient.Close();
            });
            
        }
    }
}
