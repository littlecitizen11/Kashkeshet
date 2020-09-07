using Common;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ClientDemo
{
    class Program
    {
        private static Serializations serializations = new Serializations();
        private static void ReceiveData(TcpClient client)
        {
            NetworkStream ns = client.GetStream();
            ns.Flush();
            var receivedBytes = new byte[client.ReceiveBufferSize];
            int byte_count;
            try
            {
                while ((byte_count = ns.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
                {
                    //ns.Read(receivedBytes, 0, receivedBytes.Length);
                    string data = (string)serializations.ByteArrayToObject(receivedBytes);
                    Console.WriteLine("new msg : " + data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public static void StartClient()
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry("127.0.0.1");
                IPAddress ipAddress = host.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 1234);
                using (TcpClient client = new TcpClient())
                {
                    try
                    {
                        while (client.Connected == false)
                        {
                            try
                            {
                                client.Connect(remoteEP);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Waiting for server...");
                            }
                        }
                        string messagetosend;
                        byte[] bytes;
                        Thread thread;
                        using (NetworkStream networkStream = client.GetStream())
                        {
                            Console.WriteLine("Enter Username");
                            string username = Console.ReadLine();
                            bytes = serializations.ObjectToByteArray(username);
                            networkStream.Write(bytes, 0, bytes.Length);
                            Console.WriteLine("Enter message to send");
                            thread = new Thread(() => ReceiveData((client)));
                            thread.Start();
                            while ((messagetosend = Console.ReadLine()) != "exit")
                            {
                                bytes = serializations.ObjectToByteArray(messagetosend);
                                networkStream.Write(bytes, 0, bytes.Length);
                                //bytes = new byte[client.ReceiveBufferSize];
                                //networkStream.Read(bytes, 0, bytes.Length);
                                //string newmessage = (string)serializations.ByteArrayToObject(bytes);
                                //Console.WriteLine("message arrived :" + newmessage);
                                Console.WriteLine("Enter message to send");
                            }
                        }
                        //client.Client.Shutdown(SocketShutdown.Send);
                        thread.Join();
                        client.Close();
                        client.Dispose();
                        Console.WriteLine("disconnected");


                    }
                    catch (ArgumentNullException ane)
                    {
                        Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                    }
                    catch (SocketException se)
                    {
                        Console.WriteLine("SocketException : {0}", se.ToString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Unexpected exception : {0}", e.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
    
    static void Main(string[] args)
        {
            StartClient();
        }
    }
}
