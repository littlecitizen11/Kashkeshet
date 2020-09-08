using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Common
{
    public class Serializations
    {
        public byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            IFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public object ByteArrayToObject(byte[] arr)
        {
            if (arr == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            
            using (MemoryStream ms = new MemoryStream(arr))
            {
                ms.Position=0;
                return bf.Deserialize(ms);
            }
        }
    }
}
