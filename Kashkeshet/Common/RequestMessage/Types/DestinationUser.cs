using System;
using System.Collections.Generic;

namespace Common
{
    [Serializable]
    public class DestinationUser:IMessageDestination
    {
        public List<string> DestUser { get; set; }
        public DestinationUser(List<string> destUser)
        {
            DestUser = destUser;
        }

        public List<string> Get()
        {
            return DestUser;
        }
    }
}
