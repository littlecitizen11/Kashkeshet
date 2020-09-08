using Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ServerKashkeshet
{
    public class ReceiveData
    {
        private TcpClient _client;
        private Dictionary<TcpClient, string> _clients;
        private Serializations _serializations;
        private NetworkStream _networkStream;
        public ReceiveData(TcpClient client, Dictionary<TcpClient, string> clients, Serializations serializations)
        {
            _client = client;
            _clients = clients;
            _serializations = serializations;
            Receive();
        }
        public void Receive()
        {
            NetworkStream _networkStream = _client.GetStream();
            _networkStream.Flush();
            var receivedBytes = new byte[_client.ReceiveBufferSize];
            int byte_count;
            try
            {
                while ((byte_count = _networkStream.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
                {
                    IMessage data = (IMessage)_serializations.ByteArrayToObject(receivedBytes);
                    Type thisType = this.GetType();
                    string methodname = "Receive" + data.MessageType;
                    Console.WriteLine(methodname);
                    this.GetType().GetMethod(methodname).Invoke(this,new[] { data });
                    Broadcaster broadcast = new Broadcaster(_clients);
                    byte[] bytes = new byte[_client.ReceiveBufferSize];
                    bytes = _serializations.ObjectToByteArray(data);
                    _networkStream.Flush();
                    receivedBytes = new byte[_client.ReceiveBufferSize];
                    //broadcast.Broadcast(bytes);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void ReceiveText(IMessage data)
        {
            Message<string> dataConvert = (Message<string>)data;
            Console.WriteLine(" {0} : {1}", dataConvert.ClientUser.UserName, dataConvert.ClientMessage);
            Broadcaster br = new Broadcaster(_clients);
            br.Broadcast(_serializations.ObjectToByteArray(data));
        }
        public void ReceiveImage(IMessage data)
        {
            Message<Bitmap> dataConvert = (Message<Bitmap>)data;
            dataConvert.ClientMessage.Save("shit.jpeg", ImageFormat.Jpeg);
        }
        public void ReceiveTextToDest(IMessage data)
        {
            Message<string> dataConvert = (Message<string>)data;
            byte[] bytes = new byte[_client.ReceiveBufferSize];
            bytes = _serializations.ObjectToByteArray(data);
            TcpClient myKey = _clients.FirstOrDefault(x => x.Value == (string)dataConvert.MessageDestination.Get()).Key;
            NetworkStream stream = myKey.GetStream();
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
