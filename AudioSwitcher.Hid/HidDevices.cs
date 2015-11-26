using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AudioSwitcher.Hid
{
    public static class HidDevices
    {

        public static void DoNothing()
        {
            var devices = new List<NativeMethods.DeviceInfoData>();
            Guid hidGuid; NativeMethods.HidD_GetHidGuid(out hidGuid);
            var deviceInfoSet = NativeMethods.SetupDiGetClassDevs(ref hidGuid, null, IntPtr.Zero, NativeMethods.DIGCF.Present | NativeMethods.DIGCF.DeviceInterface);

            if (deviceInfoSet.ToInt64() != -1)
            {
                var deviceInfoData = new NativeMethods.DeviceInfoData();
                deviceInfoData.Size = Marshal.SizeOf(deviceInfoData);
                deviceInfoData.DevInst = 0;
                deviceInfoData.ClassGuid = Guid.Empty;
                deviceInfoData.Reserved = IntPtr.Zero;

                var deviceIndex = 0;

                while (NativeMethods.SetupDiEnumDeviceInfo(deviceInfoSet, deviceIndex, ref deviceInfoData))
                {
                    deviceIndex += 1;

                    var deviceInterfaceData = new NativeMethods.SP_DEVICE_INTERFACE_DATA();
                    deviceInterfaceData.cbSize = Marshal.SizeOf(deviceInterfaceData);
                    var deviceInterfaceIndex = 0;

                    while (NativeMethods.SetupDiEnumDeviceInterfaces(deviceInfoSet, ref deviceInfoData, ref hidClass,
                        deviceInterfaceIndex, ref deviceInterfaceData))
                    {
                        deviceInterfaceIndex++;
                        var devicePath = GetDevicePath(deviceInfoSet, deviceInterfaceData);
                        var description = GetBusReportedDeviceDescription(deviceInfoSet, ref deviceInfoData) ??
                                          GetDeviceDescription(deviceInfoSet, ref deviceInfoData);
                        devices.Add(new DeviceInfo { Path = devicePath, Description = description });
                    }
                }
                NativeMethods.SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }
        }
    }
}
