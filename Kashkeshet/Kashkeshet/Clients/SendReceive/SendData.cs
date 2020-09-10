using Common;
using Common.Display;
using Common.Displayer;
using Common.RequestMessage.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
            byte[] receivedBytes = new byte[8192];
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
            foreach (string client in _clientsProperties._clients.Where(x=>x!=_clientsProperties.User.UserName))
            {
                _displayer.Print(client);
            }
        }

        public void SendPrivateMessage()
        {
            List<string> listofprivates = new List<string>();
            PrintClients();
            _displayer.Print("Enter From List");
            listofprivates.Add(Console.ReadLine());
            listofprivates.Add(_clientsProperties.User.UserName);
            IChat chat = IsChatExist(listofprivates, ChatTypes.Private);
            if (chat == null)
            {
                //CreateNewChat(new DestinationUser(listofprivates), ChatTypes.Private);
                chat = CreateNewChat(new DestinationUser(listofprivates), ChatTypes.Private);

            }
            _clientsProperties.currentChat = chat;
            SendTextMessage(MessageType.TextToDest, chat);

        }
        public void SendTextMessage(MessageType msgType, IChat messageDestination)
        {
            //Console.Clear();
            string messagetosend;
            Queue<string> checker;
            if (_clientsProperties._chatsByHistory.TryGetValue(messageDestination.Id, out checker))
            {
                while (checker.Count > 0)
                {
                    _displayer.Print((checker.Dequeue()));
                }
            }
            _displayer.Print("Enter message to send");
            while ((messagetosend = Console.ReadLine()) != "exit")
            {
                Message<string> newmessage = new Message<string>(messagetosend, _clientsProperties.User, msgType, messageDestination);
                byte[] bytes = serializations.ObjectToByteArray(newmessage);
                _clientsProperties.client.GetStream().Write(bytes, 0, bytes.Length);
                _displayer.Print("Enter message to send");
            }
            _clientsProperties.currentChat = null;
        }

        public IChat IsChatExist(List<string> destination, ChatTypes chatType)
        {
            foreach (IChat chat in _clientsProperties._chats)
            {
                if (chat.Destination.Get().Any(destination.Contains) && chat.ChatType == chatType)
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
            catch(Exception)
            {
                _displayer.Print("Couldnt Get Chats........"+_clientsProperties._chats.Count);
            }

        }
        public void SendGroupMessage()
        {
            PrintClients();
            _displayer.Print("Enter From List, exit to finish");
            List<string> desinations = new List<string>();
            desinations.Add(_clientsProperties.User.UserName);
            string destination;
            while ((destination = Console.ReadLine()) != "exit")
                desinations.Add(destination);

            IChat chat = IsChatExist(desinations, ChatTypes.Group);
            if(chat==null)
                chat = CreateNewChat(new DestinationUser(desinations), ChatTypes.Group);
            SendTextMessage(MessageType.TextToDest, chat);
            _clientsProperties.currentChat = chat;
        }

        public void ShowActiveChats()
        {
            string st;
            foreach (var chat in _clientsProperties._chats)
            {
                st = "";
                st+=("Chat ID :"+ chat.Id+" "+ chat.ChatType.ToString()+"\n");

                st += ("With : ");
                foreach (var i in chat.Destination.Get())
                {
                    st += (i+" ");
                }
                _displayer.Print(st);
            }
            
        }


    }
    }
