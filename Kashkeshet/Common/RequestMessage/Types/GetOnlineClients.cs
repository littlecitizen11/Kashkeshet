using System;
using System.Collections.Generic;
using System.Text;

namespace Common.RequestMessage.Types
{
    [Serializable]
    public class GetOnlineClients
    {
        public List<string> Clients { get; set; }
        public GetOnlineClients(List<string> clients)
        {
            Clients = clients;
        }
    }
}
