using Common.Displayer;
using System;

namespace Common.Display
{
    public class Displayer:IDisplayer
    {
        public void Print(string st)
        {
            Console.WriteLine(st);
        }
    }
}
