using Common;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerKashkeshet
{
    public class ReceiveData
    {
        private TcpClient _client;
        private Dictionary<TcpClient, string> _clients;
        private Serializations _serializations;
        private List<IChat> _chats;
        public ReceiveData(TcpClient client, Dictionary<TcpClient, string> clients, Serializations serializations, List<IChat> chats)
        {
            _chats = chats;
            _client = client;
            _clients = clients;
            _serializations = serializations;
            Receive();
        }
        public void Receive()
        {
            var receivedBytes = new byte[8192];
            int byte_count;
            try
            {
                NetworkStream _networkStream = _client.GetStream();
                _client.GetStream().Flush();
                _client.NoDelay = true;
                _client.Client.NoDelay = true;
                _networkStream.Flush();
                while ((byte_count = _networkStream.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
                {
                    IMessage data = (IMessage)_serializations.ByteArrayToObject(receivedBytes);
                    ReceiveTypes receiveTypes = new ReceiveTypes(_client,_clients,_chats);
                    Task t = new Task(() =>
                    receiveTypes.GetType().GetMethod("Receive" + data.MessageType).Invoke(receiveTypes, new[] { data }));
                    t.RunSynchronously();
                    _client.GetStream().Flush();
                    _client.NoDelay = true;
                    _client.Client.NoDelay = true;
                    receivedBytes = new byte[8192];
                    t.Wait();
                    t.Dispose();
                    byte[] bytes = new byte[8192];
                    bytes = _serializations.ObjectToByteArray(data);
                    _networkStream.Flush();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
