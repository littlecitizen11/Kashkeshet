using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{

    public interface IChat
    {
        public Guid Id { get; set; }
        public ChatTypes ChatType { get; set; }
        public IMessageDestination Destination { get; set; }

    }
}
