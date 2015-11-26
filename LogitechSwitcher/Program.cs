using System;
using AudioSwitcher.Hid;

namespace LogitechSwitcher
{
    class Program
    {
        static void Main(string[] args)
        {
            HidDevices.DoNothing();

            Console.ReadKey();
        }
    }
}
