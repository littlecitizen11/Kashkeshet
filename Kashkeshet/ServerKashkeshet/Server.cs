using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Common;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace ServerKashkeshet
{
    public class Server
    {
        static readonly object _lock = new object();
        static readonly Dictionary<TcpClient, string> list_clients = new Dictionary<TcpClient,string>();
        private Serializations serializations = new Serializations();

        public static void Broadcast(byte[] bytes)
        {
            lock (_lock)
            {
                foreach (TcpClient c in list_clients.Keys)
                {
                    if (c.Connected)
                    {
                        NetworkStream stream = c.GetStream();

                        stream.Write(bytes, 0, bytes.Length);
                    }
                }
            }
        }
        private void ReceiveData(TcpClient client)
        {
            NetworkStream networkStream = client.GetStream();
            networkStream.Flush();
            var receivedBytes = new byte[client.ReceiveBufferSize];
            int byte_count;
            try
            {
                while ((byte_count = networkStream.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
                {
                    byte[] bytes = new byte[client.ReceiveBufferSize];
                    string data = (string)serializations.ByteArrayToObject(receivedBytes);
                    bytes = serializations.ObjectToByteArray(data);
                    //Broadcast(bytes);
                    data = (string)serializations.ByteArrayToObject(bytes);
                    Console.WriteLine("client {0} said : {1}",list_clients[client],data);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void StartServer()
        {
            IPHostEntry host = Dns.GetHostEntry("127.0.0.1");
            IPAddress ipAddress = host.AddressList[0];
            int port = 1234;
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
            try
            {
                var server = new TcpListener(ipAddress, port);
                server.Start();
                Console.WriteLine("Waiting for a connection...");
                Serializations serializations = new Serializations();
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Task t = Task.Factory.StartNew(() =>
                    {
                        using (NetworkStream networkStream = client.GetStream())
                        {
                            try
                            {
                                var receivedBytes = new byte[client.ReceiveBufferSize];
                                networkStream.Read(receivedBytes, 0, receivedBytes.Length);
                                string data = (string)serializations.ByteArrayToObject(receivedBytes);
                                list_clients.Add(client,data);
                                Console.WriteLine("Hello " + data);

                            }
                            catch (Exception e)
                            {

                                Console.WriteLine(e.ToString());
                            }
                            ReceiveData(client);
                        }
                        Console.WriteLine("{0} disconnected",list_clients[client]);
                        lock (_lock) {
                            list_clients.Remove(client);
                        }
                        client.Dispose();
                        client.Close();

                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\n Press any key to continue...");
            Console.ReadKey();
        }
    }
}
