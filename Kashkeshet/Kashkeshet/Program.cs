using System;
using System.Runtime.InteropServices;

namespace Kashkeshet
{
    class Program
    {
        static void Main(string[] args)
        {
            Client c = new Client();
            c.StartClient();
            Console.Read();
        }
    }
}
