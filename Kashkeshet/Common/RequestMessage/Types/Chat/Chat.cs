using Common.RequestMessage.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    [Serializable]

    public class Chat : IChat
    {
        public Guid Id { get; set; }
        public ChatTypes ChatType { get; set; }
        public IMessageDestination Destination { get; set; }

        public Chat(Guid id, IMessageDestination clients, ChatTypes chatType)
        {
            ChatType = chatType;
            Destination = clients;
            Id = id;
        }
    }
}
