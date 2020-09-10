using Common;
using Common.Displayer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kashkeshet.Clients
{
    public class SendData
    {
        private Serializations serializations = new Serializations();
        private ClientsProperties _clientsProperties;
        private IDisplayer _displayer;
        public SendData(ref ClientsProperties clientsProperties, IDisplayer displayer)
        {
            _clientsProperties = clientsProperties;
            _displayer = displayer;
        }

        public void SendUser()
        {
            byte[] bytes;
            byte[] receivedBytes = new byte[_clientsProperties.client.ReceiveBufferSize];
            _displayer.Print("Enter Username");
            string username = Console.ReadLine();
            User user = new User(username);
            Message<User> message = new Message<User>(user, MessageType.User);
            bytes = serializations.ObjectToByteArray(message);
            _clientsProperties.client.GetStream().Write(bytes, 0, bytes.Length);
            _clientsProperties.User = user;
        }
        public void PrintClients()
        {
            _displayer.Print("Connected Clients: ");
            foreach (string client in _clientsProperties._clients.Where(x => x != _clientsProperties.User.UserName))
            {
                _displayer.Print(client);
            }
        }

        public void SendPrivateMessage()
        {
            List<string> listofprivates = new List<string>();
            PrintClients();
            _displayer.Print("Enter From List");
            string clientToSend;
            while((clientToSend=Console.ReadLine())==null||!_clientsProperties._clients.Contains(clientToSend)|| clientToSend==_clientsProperties.User.UserName)
            {
                _displayer.Print("Wrong value");
            }
            listofprivates.Add(clientToSend);
            listofprivates.Add(_clientsProperties.User.UserName);
            IChat chat;
            if((chat=IsChatExist(listofprivates,ChatTypes.Private))==null)
                chat = CreateNewChat(new DestinationUser(listofprivates), ChatTypes.Private);
            _clientsProperties.currentChat = chat;
            SendTextMessage(MessageType.TextToDest, chat);
        }
        public void SendTextMessage(MessageType msgType, IChat messageDestination)
        {
            Console.Clear();
            string messagetosend;
            Queue<string> chatHistory;
            if (_clientsProperties._chatsByHistory.TryGetValue(messageDestination.Id, out chatHistory))
            {
                while (chatHistory.Count > 0)
                {
                    _displayer.Print((chatHistory.Dequeue()));
                }
            }
            _displayer.Print("Enter message to send - enter 'exit' to end");
            while ((messagetosend = Console.ReadLine()) != "exit")
            {
                Message<string> newmessage = new Message<string>(messagetosend, _clientsProperties.User, msgType, messageDestination);
                byte[] bytes = serializations.ObjectToByteArray(newmessage);
                _clientsProperties.client.GetStream().Write(bytes, 0, bytes.Length);
            }
            _clientsProperties.currentChat = null;
        }

        public IChat IsChatExist(List<string> destination, ChatTypes chatType)
        {
            foreach (IChat chat in _clientsProperties._chats)
            {
                if (((chat.Destination.Get().Intersect(destination)).Count()==destination.Count()) && chat.ChatType == chatType)
                    return chat;
            }
            return null;
        }
        public IChat CreateNewChat(IMessageDestination destination, ChatTypes chatType)
        {
            Chat chat = new Chat(Guid.NewGuid(), destination, chatType);
            Message<Chat> newmessage = new Message<Chat>(chat, _clientsProperties.User, MessageType.CreateChat);
            _clientsProperties._chats.Add(chat);
            _clientsProperties._chatsByHistory.Add(chat.Id, new Queue<string>());
            byte[] bytes = serializations.ObjectToByteArray(newmessage);
            _clientsProperties.client.GetStream().Write(bytes, 0, bytes.Length);
            return chat;
        }
        public void SendGlobalMessage()
        {
            try
            {
                _clientsProperties.currentChat = _clientsProperties._chats.FirstOrDefault();
                SendTextMessage(MessageType.Text, _clientsProperties._chats.FirstOrDefault());
            }
            catch (Exception e)
            {
                _displayer.Print(e.ToString());
            }

        }
        public void SendGroupMessage()
        {
            PrintClients();
            _displayer.Print("Enter From List, exit to finish");
            List<string> desinations = new List<string>();
            desinations.Add(_clientsProperties.User.UserName);
            string destination;
            while ((destination = Console.ReadLine()) != "exit"&&_clientsProperties._clients.Contains(destination)&&destination!=_clientsProperties.User.UserName)
            {
                desinations.Add(destination);

            }
            IChat chat;
            if ((chat = IsChatExist(desinations, ChatTypes.Group)) == null)
                chat = CreateNewChat(new DestinationUser(desinations), ChatTypes.Group);
            _clientsProperties.currentChat = chat;
            SendTextMessage(MessageType.TextToDest, chat);
        }

        public void ShowActiveChats()
        {
            string st;
            foreach (IChat chat in _clientsProperties._chats)
            {
                st = "";
                st += "No. ["+_clientsProperties._chats.IndexOf(chat)+"] ";
                st += (chat.ChatType.ToString() + " ");

                st += ("With : ");
                foreach (string i in chat.Destination.Get())
                {
                    st += (i + " ");
                }
                _displayer.Print(st);
            }
            _displayer.Print("enter number of chat");
            int n;
            while (!int.TryParse(Console.ReadLine(), out n)||n<0||_clientsProperties._chats.Count<n)
            {
               _displayer.Print("Incorrect value");
            }
            _clientsProperties.currentChat = _clientsProperties._chats[n]; 
            switch (_clientsProperties._chats[n].ChatType)
            {
                case ChatTypes.Global:
                    SendTextMessage(MessageType.Text, _clientsProperties._chats[n]);
                    break;
                case ChatTypes.Private:
                    SendTextMessage(MessageType.TextToDest, _clientsProperties._chats[n]);
                    break;
                case ChatTypes.Group:
                    SendTextMessage(MessageType.TextToDest, _clientsProperties._chats[n]);
                    break;
                default:
                    break;
            }


        }


    }
}
