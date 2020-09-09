using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Common;
using System.Threading.Tasks;
using Common.RequestMessage.Types;
using System.Linq;
using Serilog;

namespace ServerKashkeshet
{
    public class Server
    {
        static readonly object _lock = new object();
        static readonly Dictionary<TcpClient, string> _connectedClients = new Dictionary<TcpClient, string>();
        private Serializations serializations = new Serializations();
        ILogger _logger;
        private List<IChat> _chats = new List<IChat>();
        

        public void SendOnlineClients()
        {
            GetOnlineClients getOnlineClients = new GetOnlineClients(new List<string>());
            getOnlineClients.Clients = _connectedClients.Values.ToList();
            Message<GetOnlineClients> message = new Message<GetOnlineClients>(getOnlineClients, null, MessageType.GetOnlineClients);
            Broadcaster br = new Broadcaster(_connectedClients);
            br.Broadcast(serializations.ObjectToByteArray(message));
        }
        public void CurrentGlobalChat()
        {
            Message<Chat> message = new Message<Chat>((Chat)_chats[0], null, MessageType.CreateChat);
            Broadcaster br = new Broadcaster(_connectedClients);
            foreach (TcpClient c in _connectedClients.Keys)
            {
                if (c.Connected)
                {
                    c.GetStream().Write(serializations.ObjectToByteArray(message), 0, serializations.ObjectToByteArray(message).Length);
                }
            }
        }
        public void GetUserName(TcpClient _client)
        {
            try
            {
                var receivedBytes = new byte[_client.ReceiveBufferSize];
                _client.GetStream().Read(receivedBytes, 0, receivedBytes.Length);
                var data = (Message<User>)serializations.ByteArrayToObject(receivedBytes);
                lock (_lock)
                 _connectedClients.Add(_client, data.ClientUser.UserName);
                _chats[0].Destination = new DestinationGlobal(_connectedClients.Values.ToList());
                SendOnlineClients();
                CurrentGlobalChat();
                Task.Delay(100);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void StartServer(ILogger logger)
        {
            try
            {
                var serverInit = new ServerInitializer();
                var server = serverInit.Initialize();
                _logger = logger;
                server.Start();
                _chats.Add(new Chat(Guid.NewGuid(), new DestinationGlobal(new List<string>()),ChatTypes.Global));
                _logger.Information("Server Init");
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Task t = Task.Factory.StartNew(() =>
                    {
                        using (NetworkStream networkStream = client.GetStream())
                        {
                            GetUserName(client);
                            new ReceiveData(client, _connectedClients, serializations,_chats);
                        }
                        _logger.Information("{0} disconnected", _connectedClients[client]);
                        lock (_lock)
                        {
                            _connectedClients.Remove(client);
                        }
                        client.Dispose();
                        client.Close();
                    });
                }
            }
            catch (Exception e)
            {
                _logger.Error("Server is down because : "+e.ToString());
            }
            Console.ReadKey();
        }
    }
}
