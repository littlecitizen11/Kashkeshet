using Common.Displayer;
using System;
using System.Collections.Generic;
using System.Text;

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
