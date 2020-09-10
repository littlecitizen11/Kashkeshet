using Common;
using Common.RequestMessage.Types;
using Serilog;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Sockets;
using System.Text;

namespace ServerKashkeshet.Server
{
    public class ServerProperties
    {
        public Dictionary<TcpClient, string> _connectedClients { get; set; }
        public Serializations serializations { get; set; }
        public Broadcaster br { get; set; }
        public List<IChat> _chats { get; set; }
        
        public ServerProperties()
        {
            InitProps();
        }
        public void InitProps()
        {
            _connectedClients = new Dictionary<TcpClient, string>();
            serializations = new Serializations();
            _chats = new List<IChat>();
            br = new Broadcaster();
            AddGlobalChat();
        }
        public void AddGlobalChat()
        {
            _chats.Add(new Chat(Guid.NewGuid(), new DestinationGlobal(new List<string>()), ChatTypes.Global)); //init for global chat

        }
    }
}
