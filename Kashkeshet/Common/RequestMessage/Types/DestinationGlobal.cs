using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Common.RequestMessage.Types
{
    [Serializable]

    public class DestinationGlobal:IMessageDestination
    {
        public List<string> Clients;
        public DestinationGlobal(List<string> clients)
        {
            Clients = clients;
        }

        public List<string> Get()
        {
            return Clients;
        }
    }
}
