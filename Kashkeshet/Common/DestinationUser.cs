using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    [Serializable]
    public class DestinationUser:IMessageDestination
    {
        public string DestUser { get; set; }
        public DestinationUser(string destUser)
        {
            DestUser = destUser;
        }

        public object Get()
        {
            return DestUser;
        }
    }
}
