using System;
using System.Globalization;
using System.Text.RegularExpressions;
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

        public IHidDevice Device
        {
            get
            {
                return _device;
            }
        }

        public int PId
        {
            get
            {
                try
                {
                    return Int32.Parse(Regex.Matches(Device.DevicePath, "pid_([0-9A-Fa-f]{3,4})")[0].Groups[1].Value, NumberStyles.HexNumber);
                }
                catch
                {
                    return -1;
                }
            }
        }

        public int VId
        {
            get
            {
                try
                {
                    return Int32.Parse(Regex.Matches(Device.DevicePath, "vid_([0-9A-Fa-f]{3,4})")[0].Groups[1].Value, NumberStyles.HexNumber);
                }
                catch
                {
                    return -1;
                }
            }
        }

        public UsbDevice(IHidDevice device)
        {
            _device = device;
            _device.Inserted += DeviceInsert;
            _device.Removed += DeviceRemove;
            _device.MonitorDeviceEvents = true;

            if (device.IsConnected)
                DeviceInsert();
        }

        //private async void ReadThread(object state)
        //{
        //    if (!Device.IsOpen)
        //        Device.OpenDevice();

        //    _isActive = true;

        //    while (_isActive)
        //    {
        //        try
        //        {
        //            var result = await _device.ReadReportAsync();

        //            if (result.ReadStatus == HidDeviceData.ReadStatus.Success)
        //                ProcessReadResult(result);


        //            //).ContinueWith(x =>
        //            //{
        //            //    if (x.Result.ReadStatus == HidDeviceData.ReadStatus.Success)
        //            //        Console.WriteLine(BitConverter.ToString(x.Result.Data));
        //            //}).Start();
        //        }
        //        catch
        //        {

        //        }
        //        //{
        //        //    if (x.IsCompleted)
        //        //        ProcessReadResult(x.Result);
        //        //});
        //    }

        //    Device.CloseDevice();
        //}

        protected virtual void ProcessReadResult(HidReport result)
        {
            //if (result.ReportId == 0 || result.Data.Length < 18 || result.Data.Length > 22)
            //    return;

            if (result.ReadStatus == HidDeviceData.ReadStatus.Success)
                Console.WriteLine("{0}-{1}", BitConverter.ToString(new byte[] { result.ReportId }), BitConverter.ToString(result.Data));
        }

        public void Dispose()
        {
            Dispose(true);
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

        private void DeviceInsert()
        {
            OnInserted();

            if (!_isActive)
            {
                _device.ReadReport(OnReport);
                //new Thread(ReadThread).Start();
                //ThreadPool.QueueUserWorkItem(ReadThread);
            }
        }

        private void OnReport(HidReport report)
        {
            ProcessReadResult(report);
        }

        private void DeviceRemove()
        {
            _isActive = false;
            OnRemoved();
        }

        private static int GetNumberFromRegex(string input, string pattern)
        {
            var match = Regex.Match(input, pattern, RegexOptions.IgnoreCase);

            var num = 0;

            if (match.Success)
            {
                num = int.Parse(match.Groups[0].Value, System.Globalization.NumberStyles.HexNumber);
            }

            return num;
        }
    }
}
