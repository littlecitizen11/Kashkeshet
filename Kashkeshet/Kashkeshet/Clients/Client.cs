using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.RequestMessage.Types;
using MenuBuilder;

namespace Kashkeshet
{
    public class Client
    {
        private Serializations serializations = new Serializations();
        public User User { get; set; }
        private List<IChat> _chats = new List<IChat>();
        private IChat currentChat;
        private List<string> _clients;
        private List<Group> _groups = new List<Group>();

        private TcpClient client;
        private void ReceiveData(TcpClient client)
        {
            byte[] receivedBytes = new byte[4096];
            int byte_count;
            try
            {
                while ((byte_count = client.GetStream().Read(receivedBytes, 0, receivedBytes.Length)) > 0)
                {
                    var data = (IMessage)serializations.ByteArrayToObject(receivedBytes);
                    Console.WriteLine($"type : {data.MessageType}");
                    Task t = new Task(() =>
                    this.GetType().GetMethod("Receive" + data.MessageType).Invoke(this, new[] { data }));
                    t.RunSynchronously();
                    //t.Start();

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
        /*public void AddToGroup()
        {
            PrintClients();
            string username;
            string nameOfGroup;
            List<User> usersInGroup = new List<User>();
            Console.WriteLine("Enter name of group");
            nameOfGroup = Console.ReadLine();
            Console.WriteLine("Enter user to add");
            while ((username = Console.ReadLine()) != "exit")
            {
                User usertoadd = new User(username);
                if (!usersInGroup.Contains(usertoadd))
                    usersInGroup.Add(usertoadd);
                Console.WriteLine("Enter user to add");
            }
        }*/
        public void ReceiveGetOnlineClients(IMessage data)
        {
            Message<GetOnlineClients> convertData = (Message<GetOnlineClients>)data;
            _clients = convertData.ClientMessage.Clients;


        }
        public void PrintClients()
        {
            Console.WriteLine("Connected Clients: ");
            foreach (var item in _clients)
            {
                Console.WriteLine(item);
            }
        }

        public void ReceiveText(IMessage data)
        {
            Message<string> convertData = (Message<string>)data;
            if (currentChat != null && convertData.MessageDestination.Id == currentChat.Id)
                Console.WriteLine("{0} : {1}", convertData.ClientUser.UserName, convertData.ClientMessage);
        }
        public void SendTextMessage(MessageType msgType, IChat messageDestination)
        {
            //Console.Clear();
            string messagetosend;
            Console.WriteLine("Enter message to send");
            while ((messagetosend = Console.ReadLine()) != "exit")
            {
                Message<string> newmessage = new Message<string>(messagetosend, User, msgType, messageDestination);
                byte[] bytes = serializations.ObjectToByteArray(newmessage);
                client.GetStream().Write(bytes, 0, bytes.Length);
                Console.WriteLine("Enter message to send");
            }
            currentChat = null;
        }
        public void GetOnlineClients()
        {
            GetOnlineClients getOnlineClients = new GetOnlineClients(new List<string>());
            Message<GetOnlineClients> message = new Message<GetOnlineClients>(getOnlineClients, User, MessageType.GetOnlineClients);
            byte[] bytes = serializations.ObjectToByteArray(message);
            client.GetStream().Write(bytes, 0, bytes.Length);

        }
        /*public void PrintGroups()
        {
            foreach (var item in _groups)
            {

            }
        }*/
        /*        public void SendGroupMessage()
                {
                    PrintGroups();
                    Console.WriteLine("Enter From List");
                    string destination = Console.ReadLine();
                    SendTextMessage(MessageType.TextToDest, new DestinationUser(destination));
                }*/
        public void CreateNewChat(IMessageDestination destination, ChatTypes chatType)
        {
            Chat chat = new Chat(Guid.NewGuid(), destination, chatType);
            Message<Chat> newmessage = new Message<Chat>(chat, User, MessageType.CreateChat);
            _chats.Add(chat);
            byte[] bytes = serializations.ObjectToByteArray(newmessage);
            client.GetStream().Write(bytes, 0, bytes.Length);
        }
        public void SendPrivateMessage()
        {
            List<string> listofprivates = new List<string>();
            PrintClients();
            Console.WriteLine("Enter From List");
            listofprivates.Add(Console.ReadLine());
            listofprivates.Add(User.UserName);
            Console.WriteLine("call EXIST");
            IChat chat = IsChatExist(listofprivates, ChatTypes.Private);
            if (chat == null)
            {
                Console.WriteLine("CREATE CHAT");
                CreateNewChat(new DestinationUser(listofprivates), ChatTypes.Private);
                Console.WriteLine("call EXIST AFTER CREATE");
                chat = IsChatExist(listofprivates, ChatTypes.Private);

            }
            currentChat = chat;
            Console.WriteLine("user {0} , Guid {1} : ", User.UserName, chat.Id);
            SendTextMessage(MessageType.TextToDest, chat);

        }
        public IChat IsChatExist(List<string> name, ChatTypes chatType)
        {
            foreach (var item in _chats)
            {
/*                Console.WriteLine("number of chats in :" + _chats.Count);
                Console.WriteLine(_chats.IndexOf(item));
                foreach (var i in item.Destination.Get())
                {
                    Console.Write("item is : " + i);
                }
                Console.WriteLine();
                foreach (var i in name)
                {
                    Console.Write("name is : " + i);

                }*//*
                Console.WriteLine();*/

                //Console.WriteLine(item.Destination.Get());
                if (item.Destination.Get().Any(name.Contains)&&item.ChatType==chatType)
                    return item;
            }
            return null;
        }
        /*private void ReceiveNewText()
        {
            var receivedBytes = new byte[client.ReceiveBufferSize];
            int byte_count;
            try
            {
                Console.WriteLine("Yuval is the man");
                while (true)
                {
                    byte_count = client.GetStream().Read(receivedBytes, 0, receivedBytes.Length);
                    Console.WriteLine("Yuval is not the man");
                    var data = (Message<string>)serializations.ByteArrayToObject(receivedBytes);
                    if (data.ClientMessage == "Exit")
                        break;
                    this.GetType().GetMethod("ReceiveText").Invoke(this, new[] { data });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }*/
        public void SendGlobalMessage()
        {
            currentChat = _chats[0];
            SendTextMessage(MessageType.Text, _chats[0]);

        }
        public void SendUser()
        {
            byte[] bytes;
            Console.WriteLine("Enter Username");
            string username = Console.ReadLine();
            User = new User(username);
            Message<User> message = new Message<User>(User, MessageType.User);
            bytes = serializations.ObjectToByteArray(message);
            client.GetStream().Write(bytes, 0, bytes.Length);
        }
        public void StartClient()
        {
            try
            {
                ClientInitializer init = new ClientInitializer();
                client = init.Initialize();
                Thread thread;

                SendUser();
                thread = new Thread(() => ReceiveData((client))); //thread that receiving data in background all the time
                thread.Start();
                thread.IsBackground = true;

                MenuBuilt menuBuilt = new MenuBuilt(this);
                menuBuilt.Execute();

                thread.Join();
                client.Close();
                client.Dispose();
                Console.WriteLine("disconnected");
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
        }
    }
}
