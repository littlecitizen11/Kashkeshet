using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{

    public interface IMessage
    {
        public User ClientUser { get; set; }
        public MessageType MessageType { get; set; }
    }
}
