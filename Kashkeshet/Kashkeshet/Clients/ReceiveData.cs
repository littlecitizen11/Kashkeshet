using Common;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Kashkeshet.Clients
{
    public class ReceiveData
    {
        private Serializations serializations = new Serializations();
        public User User { get; set; }
        private List<IChat> _chats = new List<IChat>();
        private IChat _currentChat;
        private List<string> _clients;
        private List<Group> _groups = new List<Group>();
        private TcpClient client;
        public ReceiveData(User user, List<IChat> chats, IChat currentChat, List<string> clients, List<Group> groups, TcpClient client)
        {
            _chats = chats;
            _currentChat = currentChat;
        }
        public void Receive(TcpClient client)
        {
            byte[] receivedBytes = new byte[4096];
            int byte_count;
            try
            {
                while ((byte_count = client.GetStream().Read(receivedBytes, 0, receivedBytes.Length)) > 0)
                {
                    var data = (IMessage)serializations.ByteArrayToObject(receivedBytes);
                    Console.WriteLine($"type : {data.MessageType}");
                    ReceiveTypes receiveTypes = new ReceiveTypes(User,_chats,_currentChat,_clients);
                    Task t = new Task(() =>
                    receiveTypes.GetType().GetMethod("Receive" + data.MessageType).Invoke(receiveTypes, new[] { data }));
                    t.RunSynchronously();
                    client.GetStream().Flush();
                    client.NoDelay = true;
                    client.Client.NoDelay = true;
                    receivedBytes = new byte[4096];
                    t.Wait();
                    t.Dispose();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
