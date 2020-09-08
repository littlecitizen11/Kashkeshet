using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ServerKashkeshet
{
    public class Broadcaster
    {
        private object _lock = new object();
        private Dictionary<TcpClient, string> _clients;

        public Broadcaster(Dictionary<TcpClient, string> clients)
        {
            _clients = clients;
        }
        public void Broadcast(byte[] bytes)
        {
            lock (_lock)
            {
                foreach (TcpClient c in _clients.Keys)
                {
                    if (c.Connected)
                    {
                        NetworkStream stream = c.GetStream();
                        stream.Write(bytes, 0, bytes.Length);
                    }
                }
            }
        }
    }
}
