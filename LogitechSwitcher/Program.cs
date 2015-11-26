using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AudioSwitcher.Hid;
using AudioSwitcher.Hid.Logitech;
using HidLibrary;

namespace LogitechSwitcher
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var hidDevice in HidDevices.Enumerate())
            {

                var d = new UsbDevice(hidDevice);

                Console.WriteLine("{0}", hidDevice.DevicePath);

            }

            Console.WriteLine("Press any key to conitnue");
            Console.ReadKey();

            var g933Devices = LogitechDevices.GetG933Devices();

            if (!g933Devices.Any())
            {
                Console.WriteLine("No G933 Found");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Found {0} devices", g933Devices.Count());

            var first = g933Devices.First();
            first.PoweredOn += () =>
            {
                Console.WriteLine("Powered On");
            };

            first.PoweredOff += () =>
            {
                Console.WriteLine("Powered Off");
            };

            Console.WriteLine("Receiving...");

            Thread.Sleep(10000);

            Console.ReadKey();
        }
    }
}
