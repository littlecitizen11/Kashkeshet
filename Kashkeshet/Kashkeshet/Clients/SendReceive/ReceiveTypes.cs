using Common;
using Common.Displayer;
using Common.RequestMessage.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Kashkeshet.Clients
{
    public class ReceiveTypes
    {
        private ClientsProperties _clientsProperties;
        private IDisplayer _displayer;
        public ReceiveTypes(ref ClientsProperties clientsProperties, IDisplayer displayer)
        {
            _clientsProperties = clientsProperties;
            _displayer = displayer;
        }
        public void ReceiveCreateChat(IMessage data)
        {
            
            Message<Chat> dataConvert = (Message<Chat>)data;
            object _lock = new object();

            if (_clientsProperties._chats.Select(x => x.Id).Contains(dataConvert.ClientMessage.Id))
            {
                //those linq are for check either an Id exists in chats field and changing the value of chatsbyhistory by the Id
                _clientsProperties._chats[_clientsProperties._chats.IndexOf(_clientsProperties._chats.Find(x => x.Id == dataConvert.ClientMessage.Id))] = dataConvert.ClientMessage;
                var a = _clientsProperties._chatsByHistory[_clientsProperties._chatsByHistory.Keys.ToList()[_clientsProperties._chats.IndexOf(_clientsProperties._chats.Find(x => x.Id == dataConvert.ClientMessage.Id))]];
                lock (_lock)
                {
                    _clientsProperties._chatsByHistory.Remove(_clientsProperties._chatsByHistory.Keys.ToList()[_clientsProperties._chats.IndexOf(_clientsProperties._chats.Find(x => x.Id == dataConvert.ClientMessage.Id))]);
                    _clientsProperties._chatsByHistory.Add(dataConvert.ClientMessage.Id, a);
                }

            }
            else
            {
                lock (_lock)
                {
                    _clientsProperties._chats.Add(dataConvert.ClientMessage);
                    _clientsProperties._chatsByHistory.Add(dataConvert.ClientMessage.Id, new Queue<string>());
                }
            }
        }
        public void ReceiveTextToDest(IMessage data)
        {
            ReceiveText(data);
        }
        public void ReceiveGetOnlineClients(IMessage data)
        {
            Message<GetOnlineClients> convertData = (Message<GetOnlineClients>)data;
            _clientsProperties._clients = convertData.ClientMessage.Clients;


        }

        public void ReceiveText(IMessage data)
        {
            Message<string> convertData = (Message<string>)data;
            if (_clientsProperties.currentChat != null && convertData.MessageDestination.Id == _clientsProperties.currentChat.Id)
            {
                _displayer.Print((convertData.ClientUser.UserName + " : " + convertData.ClientMessage)); 
            }
            else
            {
               if (_clientsProperties._chatsByHistory.ContainsKey(convertData.MessageDestination.Id))
                {
                    _clientsProperties._chatsByHistory[convertData.MessageDestination.Id].Enqueue((convertData.ClientUser.UserName + " : " + convertData.ClientMessage));
                }
            }
        }
    }
}
