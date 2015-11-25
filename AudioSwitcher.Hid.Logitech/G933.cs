using System;
using System.Linq;
using HidLibrary;
using EventMap = System.Collections.Generic.Dictionary<int, System.Action<AudioSwitcher.Hid.Logitech.G933, byte[]>>;


namespace AudioSwitcher.Hid.Logitech
{
    public sealed class G933 : UsbDevice
    {

        private static readonly EventMap EventFunctionMap = new EventMap()
        {
            {0x11FF0800, (x, y) => x.ProcessPowerEvent(y)}
        };

        internal G933(IHidDevice device)
            : base(device)
        {
        }

        protected override void ProcessReadResult(HidDeviceData result)
        {
            var eventMask = Convert.ToInt32(result.Data.Take(4));

            if (EventFunctionMap.ContainsKey(eventMask))
                EventFunctionMap[eventMask](this, result.Data);
        }

        private void ProcessPowerEvent(byte[] data)
        {
            //7th byte is zero the it's not connected
            if (data[6] == 0)
                OnPoweredOff();
            else
                OnPoweredOn();
        }
    }
}
