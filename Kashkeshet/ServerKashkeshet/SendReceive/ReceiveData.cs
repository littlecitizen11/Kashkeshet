using Common;
using System;
using System.Collections.Generic;
using System.Net.Sockets;


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
            NetworkStream _networkStream = _client.GetStream();
            _networkStream.Flush();
            var receivedBytes = new byte[_client.ReceiveBufferSize];
            int byte_count;
            try
            {
                while ((byte_count = _networkStream.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
                {
                    IMessage data = (IMessage)_serializations.ByteArrayToObject(receivedBytes);
                    ReceiveTypes receiveTypes = new ReceiveTypes(_client,_clients,_chats);
                    receiveTypes.GetType().GetMethod("Receive" + data.MessageType).Invoke(receiveTypes, new[] { data });
                    byte[] bytes = new byte[_client.ReceiveBufferSize];
                    bytes = _serializations.ObjectToByteArray(data);
                    _networkStream.Flush();
                    receivedBytes = new byte[_client.ReceiveBufferSize];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
