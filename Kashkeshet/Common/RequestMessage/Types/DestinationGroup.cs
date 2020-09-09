using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Common.RequestMessage.Types
{
    [Serializable]

    public class DestinationGroup:IMessageDestination
    {
        public List<string> DestGroup { get; set; }
        public DestinationGroup(List<string> destGroup)
        {
            DestGroup = destGroup;
        }

        public List<string> Get()
        {
            return DestGroup;
        }
    }
}
