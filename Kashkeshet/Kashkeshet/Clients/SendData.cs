using Common;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Kashkeshet.Clients
{
    public class SendData
    {
        private TcpClient client;
        private Serializations serializations = new Serializations();

        public User SendUser()
        {
            byte[] bytes;
            Console.WriteLine("Enter Username");
            string username = Console.ReadLine();
            User user = new User(username);
            Message<User> message = new Message<User>(user, MessageType.User);
            bytes = serializations.ObjectToByteArray(message);
            client.GetStream().Write(bytes, 0, bytes.Length);
            return user;
        }
        public IChat SendTextMessage(MessageType msgType, IChat messageDestination,User user)
        {
            //Console.Clear();
            string messagetosend;
            Console.WriteLine("Enter message to send");
            while ((messagetosend = Console.ReadLine()) != "exit")
            {
                Message<string> newmessage = new Message<string>(messagetosend, user, msgType, messageDestination);
                byte[] bytes = serializations.ObjectToByteArray(newmessage);
                client.GetStream().Write(bytes, 0, bytes.Length);
                Console.WriteLine("Enter message to send");
            }
            return null; //null is the non chat
        }
    }
}
