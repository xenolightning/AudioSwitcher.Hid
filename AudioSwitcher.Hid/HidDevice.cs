using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace AudioSwitcher.Hid
{
    public class HidDevice : IHidDevice
    {

        private SafeFileHandle _deviceHandle;
        private FileStream _deviceStream;


        public string DevicePath { get; private set; }

        public string Manufacturer { get; private set; }

        public int MaxFeatureReportLength { get; private set; }

        public int MaxInputReportLength { get; private set; }

        public int MaxOutputReportLength { get; private set; }

        public int ProductId { get; private set; }

        public string ProductName { get; private set; }

        public int ProductVersion { get; private set; }

        public string SerialNumber { get; private set; }
        public bool IsValid { get; private set; }

        public int VendorId { get; private set; }

        public event Action<byte[]> ReportRead;

        public event Action<byte[]> FeatureRead;


        public HidDevice(string path)
        {
            DevicePath = path;
            
            CreateDeviceHandle();
            IsValid = !_deviceHandle.IsInvalid;

            if (!IsValid)
            {
                IsValid = false;
                return;
            }

            GetDeviceInformation();
            GetInfoComplete();
            CreateFileStream();
        }

        private void CreateFileStream()
        {
            _deviceStream = new FileStream(_deviceHandle, GetFileAccess(), MaxInputReportLength, true);
        }

        protected virtual FileAccess GetFileAccess()
        {
            return FileAccess.Read;
        }

        private void CreateDeviceHandle()
        {
            _deviceHandle = NativeMethods.CreateFile(
                                               DevicePath,
                                               GetFileAccess(),
                                               FileShare.ReadWrite,
                                               IntPtr.Zero,
                                               FileMode.Open,
                                               NativeMethods.EFileAttributes.Overlapped,
                                               IntPtr.Zero);
        }

        private void GetDeviceInformation()
        {
            var attributes = new NativeMethods.DeviceAttributes();
            attributes.Size = Marshal.SizeOf(attributes);

            if (!NativeMethods.HidD_GetAttributes(_deviceHandle, ref attributes))
                return;

            ProductId = attributes.ProductId;
            VendorId = attributes.VendorId;
            ProductVersion = attributes.VersionNumber;
        }

        private void GetInfoComplete()
        {
            try
            {
                var buffer = new StringBuilder(128);

                Manufacturer = NativeMethods.HidD_GetManufacturerString(_deviceHandle, buffer, buffer.Capacity) ? buffer.ToString() : "";

                ProductName = NativeMethods.HidD_GetProductString(_deviceHandle, buffer, buffer.Capacity) ? buffer.ToString() : "";

                SerialNumber = NativeMethods.HidD_GetSerialNumberString(_deviceHandle, buffer, buffer.Capacity) ? buffer.ToString() : "";

                IntPtr preparsed;
                if (NativeMethods.HidD_GetPreparsedData(_deviceHandle, out preparsed))
                {
                    NativeMethods.DeviceCapabilities caps;
                    var statusCaps = NativeMethods.HidP_GetCaps(preparsed, out caps);
                    if (statusCaps == NativeMethods.HIDP_STATUS_SUCCESS)
                    {
                        MaxInputReportLength = caps.InputReportByteLength;
                        MaxOutputReportLength = caps.OutputReportByteLength;
                        MaxFeatureReportLength = caps.FeatureReportByteLength;
                    }
                    NativeMethods.HidD_FreePreparsedData(preparsed);
                }
            }
            catch
            {
                // ignored
            }
        }

        public void Dispose()
        {
            NativeMethods.CloseHandle(_deviceHandle.DangerousGetHandle());
        }
    }
}
