using System;
using System.Runtime.InteropServices;

namespace AudioSwitcher.Hid
{
    public static class HidDevices
    {

        public static void DoNothing()
        {
            Guid hidGuid;
            NativeMethods.HidD_GetHidGuid(out hidGuid);
            var deviceInfoSet = NativeMethods.SetupDiGetClassDevs(ref hidGuid, null, 0, NativeMethods.DIGCF_PRESENT | NativeMethods.DIGCF_DEVICEINTERFACE);

            if (deviceInfoSet.ToInt64() != -1)
            {
                var deviceInfoData = CreateDeviceInfoData();
                var deviceIndex = 0;

                while (NativeMethods.SetupDiEnumDeviceInfo(deviceInfoSet, deviceIndex, ref deviceInfoData))
                {
                    deviceIndex += 1;

                    var deviceInterfaceData = new NativeMethods.DeviceInterfaceData();
                    deviceInterfaceData.cbSize = Marshal.SizeOf(deviceInterfaceData);
                    var deviceInterfaceIndex = 0;

                    while (NativeMethods.SetupDiEnumDeviceInterfaces(deviceInfoSet, ref deviceInfoData, ref hidGuid, deviceInterfaceIndex, ref deviceInterfaceData))
                    {
                        deviceInterfaceIndex++;
                        var devicePath = GetDevicePath(deviceInfoSet, deviceInterfaceData);
                        //var description = GetBusReportedDeviceDescription(deviceInfoSet, ref deviceInfoData) ??
                        //                  GetDeviceDescription(deviceInfoSet, ref deviceInfoData);
                        //devices.Add(new DeviceInfo {Path = devicePath, Description = description});

                        var test = new HidDevice(devicePath);
                        if (test.IsValid)
                        {

                            Console.WriteLine(devicePath);
                            Console.WriteLine(test.ProductId);
                        }
                    }
                }
                NativeMethods.SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }
        }

        private static NativeMethods.DeviceInfoData CreateDeviceInfoData()
        {
            var deviceInfoData = new NativeMethods.DeviceInfoData();

            deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);
            deviceInfoData.DevInst = 0;
            deviceInfoData.ClassGuid = Guid.Empty;
            deviceInfoData.Reserved = IntPtr.Zero;

            return deviceInfoData;
        }

        private static string GetDevicePath(IntPtr deviceInfoSet, NativeMethods.DeviceInterfaceData deviceInterfaceData)
        {
            var bufferSize = 0;
            var interfaceDetail = new NativeMethods.DeviceInterfaceDetailData() { Size = IntPtr.Size == 4 ? 4 + Marshal.SystemDefaultCharSize : 8 };

            NativeMethods.SetupDiGetDeviceInterfaceDetailBuffer(deviceInfoSet, ref deviceInterfaceData, IntPtr.Zero, 0, ref bufferSize, IntPtr.Zero);

            return NativeMethods.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData, ref interfaceDetail, bufferSize, ref bufferSize, IntPtr.Zero) ?
                interfaceDetail.DevicePath : null;
        }
    }
}
