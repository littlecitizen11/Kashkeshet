/*using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using Common;
using MenuBuilder;

namespace ClientDemo
{
    public class Client
    {
        private Serializations serializations = new Serializations();
        public User User { get; set; }
        private NetworkStream networkStream;
       // private TcpClient client;
        private void ReceiveData(TcpClient client)
        {
            NetworkStream ns = client.GetStream();
            ns.Flush();
            var receivedBytes = new byte[client.ReceiveBufferSize];
            int byte_count;
            try
            {
                while ((byte_count = ns.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
                {
                    var data = (Message<string>)serializations.ByteArrayToObject(receivedBytes);
                    Console.WriteLine("{0} : {1}", data.ClientUser.UserName, data.ClientMessage);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void SendTextMessage(MessageType msgType, IChat messageDestination)
        {
            Console.WriteLine("Enter message to send");
            string messagetosend;
            //byte[] bytes;

            while ((messagetosend = Console.ReadLine()) != "exit")
            {
                Message<string> newmessage = new Message<string>(messagetosend, User, msgType, messageDestination);
                byte[] bytes = serializations.ObjectToByteArray(newmessage);
                networkStream.Write(bytes, 0, bytes.Length);
                Console.WriteLine("Enter message to send");
            }
        }
        public void SendPrivateMessage()
        {
            Console.WriteLine("Enter user");
            string destination = Console.ReadLine();
            SendTextMessage(MessageType.TextToDest, new DestinationUser(destination));
        }
        public void SendGlobalMessage()
        {
            SendTextMessage(MessageType.Text, null);
        }
        public void StartClient()
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry("127.0.0.1");
                IPAddress ipAddress = host.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 1234);
                TcpClient client = new TcpClient();
                
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

                        Thread thread;
                        byte[] bytes;

                        using (networkStream = client.GetStream())
                        {
                            Console.WriteLine("Enter Username");
                            string username = Console.ReadLine();
                            User = new User(username);
                            Message<User> message = new Message<User>(User, MessageType.User);
                            bytes = serializations.ObjectToByteArray(message);
                            networkStream.Write(bytes, 0, bytes.Length);
                            thread = new Thread(() => ReceiveData((client)));
                            thread.Start();
                            Menu<int> menu = new Menu<int>();
                            menu.AddOption(1, "Send global text", SendGlobalMessage);
                            menu.AddOption(2, "Send Private msg", SendPrivateMessage);
                            MenuBuilder.RunIntMenu menu2 = new RunIntMenu();
                            menu2.Menu = menu;
                            menu2.Run();

                            //menu.ToString();
*//*                            Console.WriteLine("1 - send TXT, 2 send Image, 3 send specific ");
                            int response = int.Parse(Console.ReadLine());
                            switch (response)
                            {
                                case 1:
                                    SendTextMessage(MessageType.Text,null);
                                    break;

                                case 2:
                                    Console.WriteLine("Enter Path");
                                    Console.WriteLine(@"C:\Code\Kashkeshet\Kashkeshet\869967 (1).jpg");
                                    ImageBit b = new ImageBit(new Bitmap(@"C:\Code\Kashkeshet\Kashkeshet\869967 (1).jpg"));
                                    Message<ImageBit> newMessage = new Message<ImageBit>(b, User, MessageType.Image);
                                    byte[] bytes1 = serializations.ObjectToByteArray(newMessage);
                                    networkStream.Write(bytes1, 0, bytes1.Length);
                                    Console.ReadLine();
                                    break;

                                case 3:
                                    SendPrivateMessage(networkStream);
                                    break;
                                default: break;
                            }*//*

                        }
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
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
    }
}
*/