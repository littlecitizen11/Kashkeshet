using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    [Serializable]
    public class Message<T>: IMessage
    {
        public T ClientMessage { get; set; }
        public User ClientUser { get; set; }
        public MessageType MessageType { get; set; }
        public IChat MessageDestination { get; set; }
        public Message(T clientMessage, User user, MessageType messageType)
        {
            MessageType = messageType;
            ClientMessage = clientMessage;
            ClientUser = user;
        }
        public Message(T clientMessage, User user, MessageType messageType, IChat messageDestination)
        {
            MessageType = messageType;
            ClientMessage = clientMessage;
            ClientUser = user;
            MessageDestination = messageDestination;
        }
        public Message(User user,MessageType messageType)
        {
            MessageType = MessageType;
            ClientUser = user;
        }
    }
}
