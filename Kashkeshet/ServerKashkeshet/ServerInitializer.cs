using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerKashkeshet
{
    public class ServerInitializer
    {
        private Displayer _display=new Displayer();

        public TcpListener Initialize()
        {
            _display.Print("Initializing...");
            IPHostEntry host = Dns.GetHostEntry(ConfigurationManager.AppSettings["IP"]);
            IPAddress ipAddress = host.AddressList[0];
            int port = int.Parse(ConfigurationManager.AppSettings["Port"]);
            return new TcpListener(ipAddress, port);
        }
    }
}
