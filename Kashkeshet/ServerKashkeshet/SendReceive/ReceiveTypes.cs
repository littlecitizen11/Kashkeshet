using Common;
using Common.RequestMessage.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ServerKashkeshet
{
    public class ReceiveTypes
    {
        /// <summary>
        /// Add here function by the MessageTypes from common --- should be similar name because the reflection.
        /// </summary>
        private Serializations _serializations = new Serializations();
        private TcpClient _client;
        private Dictionary<TcpClient, string> _clients;
        private List<IChat> _chats;
        private Broadcaster _broadcaster = new Broadcaster();



        public ReceiveTypes(TcpClient client, Dictionary<TcpClient, string> clients, List<IChat> chats)
        {
            _chats = chats;
            _client = client;
            _clients = clients;

        }
        public void ReceiveText(IMessage data)
        {
            Message<string> dataConvert = (Message<string>)data;
            Console.WriteLine(dataConvert.ClientUser.UserName+" : "+dataConvert.ClientMessage);
            _broadcaster.Broadcast(_serializations.ObjectToByteArray(data),_clients);
        }
        public void ReceiveImage(IMessage data)
        {
            Message<Bitmap> dataConvert = (Message<Bitmap>)data;
            dataConvert.ClientMessage.Save("shit.jpeg", ImageFormat.Jpeg);
        }
        public void ReceiveTextToDest(IMessage data)
        {
            Message<string> dataConvert = (Message<string>)data;
            Console.WriteLine("PRIVATE - "+dataConvert.ClientUser.UserName + " : " + dataConvert.ClientMessage);

            byte[] bytes = new byte[_client.ReceiveBufferSize];
            bytes = _serializations.ObjectToByteArray(data);
            try
            {
                Dictionary<TcpClient, string> check = new Dictionary<TcpClient, string>();
                foreach (string item in dataConvert.MessageDestination.Destination.Get())
                {
                    check.Add(_clients.FirstOrDefault(x=>x.Value==item).Key, item);
                }
                _broadcaster.Broadcast(bytes,check);
            }
            catch (Exception)
            {
                Console.WriteLine("User might not exist");
                Message<string> ErrorBack = new Message<string>("User might not exist", new User("Server"), MessageType.Text);
                byte[] byteerr = new byte[_client.ReceiveBufferSize];
                byteerr = _serializations.ObjectToByteArray(ErrorBack);
                NetworkStream stream = _client.GetStream();
                stream.Write(byteerr, 0, byteerr.Length);
            }
        }
        public void ReceiveGetOnlineClients(IMessage data)
        {
            Message<GetOnlineClients> dataConvert = (Message<GetOnlineClients>)data;
            dataConvert.ClientMessage.Clients = _clients.Values.ToList();
            byte[] bytes = new byte[_client.ReceiveBufferSize];
            bytes = _serializations.ObjectToByteArray(data);
            TcpClient myKey = _clients.FirstOrDefault(x => x.Value == (string)dataConvert.ClientUser.UserName).Key;
            NetworkStream stream = myKey.GetStream();
            stream.Write(bytes, 0, bytes.Length);
        }
        public void ReceiveCreateChat(IMessage data)
        {
            byte[] bytes = new byte[_client.ReceiveBufferSize];
            bytes = _serializations.ObjectToByteArray(data);
            Message<Chat> message = (Message<Chat>) data;
            if (!_chats.Contains(message.ClientMessage))
                _chats.Add(message.ClientMessage);

            Dictionary<TcpClient, string> check = new Dictionary<TcpClient, string>();
            foreach (string item in message.ClientMessage.Destination.Get())
            {
                if(_clients.ContainsValue(item))
                    check.Add(_clients.FirstOrDefault(x => x.Value == item).Key, item);
            }
            _broadcaster.Broadcast(bytes,check);

        }
        

    }
}
