using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Display;
using Common.Displayer;
using Common.RequestMessage.Types;
using Kashkeshet.Clients;
using MenuBuilder;

namespace Kashkeshet
{
    public class Client
    {
        private Serializations serializations = new Serializations();
        private ClientsProperties _clientProperties;
        private SendData _sender;
        private IDisplayer _displayer;
        /*public void AddToGroup()
        {
            PrintClients();
            string username;
            string nameOfGroup;
            List<User> usersInGroup = new List<User>();
            Console.WriteLine("Enter name of group");
            nameOfGroup = Console.ReadLine();
            Console.WriteLine("Enter user to add");
            while ((username = Console.ReadLine()) != "exit")
            {
                User usertoadd = new User(username);
                if (!usersInGroup.Contains(usertoadd))
                    usersInGroup.Add(usertoadd);
                Console.WriteLine("Enter user to add");
            }
        }*/

/*        public void PrintGroups()
        {
            foreach (var item in _clientProperties._groups)
            {

            }
        }*/
       /*        public void SendGroupMessage()
                {
                    PrintGroups();
                    Console.WriteLine("Enter From List");
                    string destination = Console.ReadLine();
                    SendTextMessage(MessageType.TextToDest, new DestinationUser(destination));
                }*/
        public void StartClient()
        {
            try
            {
                ClientInitializer init = new ClientInitializer();
                _clientProperties = new ClientsProperties();
                _clientProperties.client = init.Initialize();
                _displayer = new Displayer();
                _sender = new SendData(ref _clientProperties, new Displayer());
                Thread thread;
                _sender.SendUser();
                ReceiveData receiveData = new ReceiveData(ref _clientProperties, new Displayer()); // runs synchronious receiving
                thread = new Thread(() => receiveData.Receive()); //thread that receiving data in background all the time
                thread.Start();
                thread.IsBackground = true;
                MenuBuilt menuBuilt = new MenuBuilt(_sender);
                menuBuilt.Execute();
                thread.Join();
                _clientProperties.client.Close();
                _clientProperties.client.Dispose();
                _displayer.Print("disconnected");
            }
            catch (ArgumentNullException ane)
            {
                _displayer.Print("ArgumentNullException : "+ ane.ToString());
            }
            catch (SocketException se)
            {
                _displayer.Print("SocketException : "+ se.ToString());
            }
            catch (Exception e)
            {
                _displayer.Print("Unexpected exception : "+ e.ToString());
            }

        }
    }
}
