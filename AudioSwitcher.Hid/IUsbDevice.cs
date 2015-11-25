using System;

namespace AudioSwitcher.Hid
{
    public interface IUsbDevice : IDisposable
    {

        bool IsDisposed { get; }

        event Action Inserted;

        event Action Removed;

        event Action PoweredOn;

        event Action PoweredOff;

    }
}
