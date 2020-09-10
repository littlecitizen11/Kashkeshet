using Common;
using Common.Display;
using Common.Displayer;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Kashkeshet.Clients
{
    public class ReceiveData
    {
        private Serializations serializations = new Serializations();
        private ClientsProperties _clientsProperties;
        private IDisplayer _displayer;
        private ReceiveTypes receiveTypes;
        public ReceiveData(ref ClientsProperties clientsProperties, IDisplayer displayer)
        {
            _clientsProperties = clientsProperties;
            _displayer = displayer;
            receiveTypes = new ReceiveTypes(ref _clientsProperties, new Displayer());
            ReceiveFirstData();
        }
        public void ReceiveFirstData()
        {
            // runs synchronious receiving - online users and global chat
            byte[] receivedBytes = new byte[_clientsProperties.client.ReceiveBufferSize];
            _clientsProperties.client.GetStream().Read(receivedBytes, 0, receivedBytes.Length);
            receiveTypes.ReceiveGetOnlineClients((IMessage)serializations.ByteArrayToObject(receivedBytes));
            receivedBytes = new byte[_clientsProperties.client.ReceiveBufferSize];
            _clientsProperties.client.GetStream().Read(receivedBytes, 0, receivedBytes.Length);
            receiveTypes.ReceiveCreateChat((IMessage)serializations.ByteArrayToObject(receivedBytes));
        }
        public void Receive()
        {
            byte[] receivedBytes = new byte[_clientsProperties.client.ReceiveBufferSize];
  
            try
            {
                while ((_clientsProperties.client.GetStream().Read(receivedBytes, 0, receivedBytes.Length)) > 0)
                {
                    IMessage data = (IMessage)serializations.ByteArrayToObject(receivedBytes);
                    Task t = new Task(() =>
                    receiveTypes.GetType().GetMethod("Receive" + data.MessageType).Invoke(receiveTypes, new[] { data }));
                    t.Start();
                    _clientsProperties.client.GetStream().Flush();
                    _clientsProperties.client.NoDelay = true;
                    _clientsProperties.client.Client.NoDelay = true;
                    receivedBytes = new byte[_clientsProperties.client.ReceiveBufferSize];

                }
            }
            catch (Exception e)
            {
                _displayer.Print("error in Receiving data in client"+e.ToString());
            }
        }
    }
}
