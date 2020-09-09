using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Common
{

    public interface IMessageDestination
    {
        public List<string> Get();
        
    }
}
