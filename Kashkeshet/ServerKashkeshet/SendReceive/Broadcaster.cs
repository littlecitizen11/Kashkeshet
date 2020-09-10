using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerKashkeshet
{
    public class Broadcaster
    {
        public Broadcaster()
        {
        }
        public void Broadcast(byte[] bytes, Dictionary<TcpClient, string> clients)
        {
            Console.WriteLine("number of clients "+clients.Count);
            Parallel.ForEach(clients.Keys, (client) =>
                {
                    if (client.Connected)
                    {
                        client.GetStream().Write(bytes, 0, bytes.Length);
                    }
                });
        }
    }
}
