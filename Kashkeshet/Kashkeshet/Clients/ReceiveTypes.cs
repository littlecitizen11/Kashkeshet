using Common;
using Common.RequestMessage.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Kashkeshet.Clients
{
    public class ReceiveTypes
    {
        private User User { get; set; }
        private List<IChat> _chats = new List<IChat>();
        private IChat _currentChat;
        private List<string> _clients;
/*        private List<Group> _groups = new List<Group>();
        private TcpClient _client;*/
        public ReceiveTypes(User user, List<IChat> chats, IChat currentChat, List<string> clients)
        {
            _chats = chats;
            _clients = clients;
            //_groups = groups;
            //_client = client;
            _currentChat = currentChat;
        }
        public void ReceiveCreateChat(IMessage data)
        {
            Console.WriteLine(User.UserName);
            Message<Chat> dataConvert = (Message<Chat>)data;
            Console.WriteLine("New Chat in ");
            if (_chats.Select(x => x.Id).Contains(dataConvert.ClientMessage.Id))
                _chats[_chats.IndexOf(_chats.Find(x => x.Id == dataConvert.ClientMessage.Id))] = dataConvert.ClientMessage;
            else _chats.Add(dataConvert.ClientMessage);
        }
        public void ReceiveTextToDest(IMessage data)
        {
            ReceiveText(data);
        }
        public void ReceiveGetOnlineClients(IMessage data)
        {
            Message<GetOnlineClients> convertData = (Message<GetOnlineClients>)data;
            _clients = convertData.ClientMessage.Clients;


        }

        public void ReceiveText(IMessage data)
        {
            Message<string> convertData = (Message<string>)data;
            if (_currentChat != null && convertData.MessageDestination.Id == _currentChat.Id)
                Console.WriteLine("{0} : {1}", convertData.ClientUser.UserName, convertData.ClientMessage);
        }
    }
}
