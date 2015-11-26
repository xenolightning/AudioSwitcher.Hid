using System;
using System.Linq;
using System.Threading;
using AudioSwitcher.Hid;
using AudioSwitcher.Hid.Logitech;

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
