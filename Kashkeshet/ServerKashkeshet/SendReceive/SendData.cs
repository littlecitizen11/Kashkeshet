using Common;
using Common.RequestMessage.Types;
using ServerKashkeshet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerKashkeshet.SendReceive
{
    public class SendData
    {
        private ServerProperties _serverProperties;
        public SendData(ServerProperties serverProperties)
        {
            _serverProperties = serverProperties;
        }

        public void SendOnlineClients()
        {
            GetOnlineClients getOnlineClients = new GetOnlineClients(new List<string>());
            getOnlineClients.Clients = _serverProperties._connectedClients.Values.ToList();
            Message<GetOnlineClients> message = new Message<GetOnlineClients>(getOnlineClients, null, MessageType.GetOnlineClients);
            _serverProperties.br.Broadcast(_serverProperties.serializations.ObjectToByteArray(message), _serverProperties._connectedClients);
        }
        public void CurrentGlobalChat()
        {
            _serverProperties._chats[0].Destination = new DestinationGlobal(_serverProperties._connectedClients.Values.ToList());
            Message<Chat> message = new Message<Chat>((Chat)_serverProperties._chats[0], null, MessageType.CreateChat);
            _serverProperties.br.Broadcast(_serverProperties.serializations.ObjectToByteArray(message), _serverProperties._connectedClients);
        }
        public void SendUpdates()
        {
            SendOnlineClients();
            CurrentGlobalChat();
        }
    }
}
