using Common;
using Common.Display;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Kashkeshet
{
    public class ClientInitializer
    {
        private readonly Displayer _display = new Displayer();
        public TcpClient Initialize()
        {
            _display.Print("Connecting To Server...");
            IPHostEntry host = Dns.GetHostEntry(ConfigurationManager.AppSettings["IP"]);
            IPAddress ipAddress = host.AddressList[0];
            int port = int.Parse(ConfigurationManager.AppSettings["Port"]);
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
            TcpClient client = new TcpClient();
            while (client.Connected == false)
            {
                try
                {
                    client.Connect(remoteEP);
                    _display.Print("Connected");
                }
                catch (Exception)
                {
                    _display.Print("Waiting for server...");
                }
            }
            return client;
        }
    }
}
