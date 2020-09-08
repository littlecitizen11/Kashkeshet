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
        static readonly Dictionary<TcpClient, string> list_clients = new Dictionary<TcpClient, string>();
        private Serializations serializations = new Serializations();
        public void GetUserName(NetworkStream _networkStream, TcpClient _client)
        {
            try
            {
                var receivedBytes = new byte[_client.ReceiveBufferSize];
                _networkStream.Read(receivedBytes, 0, receivedBytes.Length);
                var data = (Message<User>)serializations.ByteArrayToObject(receivedBytes);
                list_clients.Add(_client, data.ClientUser.UserName);
                Console.WriteLine("Hello " + data.ClientUser.UserName);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
        }
        public void StartServer()
        {
            try
            {
                var server = new ServerInitializer().Initialize();
                server.Start();
                Console.WriteLine("Server Initialized");
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Task t = Task.Factory.StartNew(() =>
                    {
                        using (NetworkStream networkStream = client.GetStream())
                        {
                            GetUserName(networkStream, client);
                            networkStream.Flush();

                            new ReceiveData(client, list_clients, serializations);
                        }
                        Console.WriteLine("{0} disconnected", list_clients[client]);
                        lock (_lock)
                        {
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
