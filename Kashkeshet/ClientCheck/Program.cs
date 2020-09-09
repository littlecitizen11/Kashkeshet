using System;
using System.Runtime.InteropServices;

namespace ClientCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            Client c = new Client();
            c.StartClient();
        }
    }
}
