using System;
using System.Threading;
using System.Threading.Tasks;
using HidLibrary;

namespace AudioSwitcher.Hid
{
    public class UsbDevice : IUsbDevice
    {
        private readonly IHidDevice _device;
        private bool _isDisposed;
        private bool _isActive;

        public bool IsDisposed
        {
            get
            {
                return _isDisposed;
            }
        }

        public event Action Inserted;

        public event Action Removed;

        public event Action PoweredOn;

        public event Action PoweredOff;

        protected IHidDevice Device
        {
            get
            {
                return _device;
            }
        }

        public UsbDevice(IHidDevice device)
        {
            _device = device;
            _device.Inserted += OnInserted;
            _device.Removed += OnRemoved;
            _device.MonitorDeviceEvents = true;

            if (device.IsConnected)
                DeviceInsert();
        }

        private void ReadThread(object state)
        {
            if (!Device.IsOpen)
                Device.OpenDevice();

            _isActive = true;

            while (!_isActive && Device.IsConnected && Device.IsOpen)
            {
                Device.ReadAsync(500).ContinueWith(x =>
                {
                    if (x.IsCompleted)
                        ProcessReadResult(x.Result);
                });
            }

            Device.CloseDevice();
        }

        protected virtual void ProcessReadResult(HidDeviceData result)
        {
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void DeviceInsert()
        {
            OnInserted();

            if (!_isActive)
                ThreadPool.QueueUserWorkItem(ReadThread);
        }

        private void DeviceRemove()
        {
            _isActive = false;
            OnRemoved();
        }

        protected virtual void OnPoweredOn()
        {
            var handler = PoweredOn;
            if (handler != null)
                handler();
        }

        protected virtual void OnPoweredOff()
        {
            var handler = PoweredOff;
            if (handler != null)
                handler();
        }

        protected virtual void OnInserted()
        {
            var handler = Inserted;
            if (handler != null)
                handler();
        }

        protected virtual void OnRemoved()
        {
            var handler = Removed;
            if (handler != null)
                handler();
        }

        protected virtual void Dispose(bool disposing)
        {
            _isActive = false;
            _isDisposed = true;
        }
    }
}
