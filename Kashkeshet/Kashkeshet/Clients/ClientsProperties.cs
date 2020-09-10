using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Kashkeshet.Clients
{
    public class ClientsProperties
    {
        public ClientsProperties()
        {

        }
        public User User { get; set; }
        public List<IChat> _chats = new List<IChat>();
        public Dictionary<Guid, Queue<string>> _chatsByHistory = new Dictionary<Guid, Queue<string>>();
        public IChat currentChat;
        public List<string> _clients;
        public List<Group> _groups = new List<Group>();
        public TcpClient client;
    }
}
