using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Common;
using System.Threading.Tasks;
using Common.RequestMessage.Types;
using System.Linq;
using Serilog;
using ServerKashkeshet.SendReceive;

namespace ServerKashkeshet.Server
{
    public class ServerKishKush
    {

        static readonly object _lock = new object();
        private static ServerProperties _serverProperties = new ServerProperties();
        private SendData _send = new SendData(_serverProperties);
        ILogger _logger;

        public void GetUserName(TcpClient _client)
        {
            try
            {
                byte[] receivedBytes = new byte[_client.ReceiveBufferSize];
                _client.GetStream().Read(receivedBytes, 0, receivedBytes.Length);
                Message<User> data = (Message<User>)_serverProperties.serializations.ByteArrayToObject(receivedBytes);
                lock (_lock)
                { _serverProperties._connectedClients.Add(_client, data.ClientUser.UserName); }
                _send.SendUpdates();

            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
            }
        }
        public void StartServer(ILogger logger)
        {
            try
            {
                ServerInitializer serverInit = new ServerInitializer();
                TcpListener server = serverInit.Initialize();
                _logger = logger;
                server.Start();
                _logger.Information("Server Init");
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Task t = Task.Factory.StartNew(() =>
                    {
                        GetUserName(client);
                        new ReceiveData(client, _serverProperties._connectedClients, _serverProperties.serializations, _serverProperties._chats);
                        _logger.Information("{0} disconnected", _serverProperties._connectedClients[client]);
                        lock (_lock)
                        {
                            _serverProperties._connectedClients.Remove(client);
                            _send.SendUpdates();
                        }
                        client.Dispose();
                        client.Close();
                    });
                }
            }
            catch (Exception e)
            {
                _logger.Error("Server is down because : " + e.ToString());
            }
            Console.ReadKey();
        }
    }
}
