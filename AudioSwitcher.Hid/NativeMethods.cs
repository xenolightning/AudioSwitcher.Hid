using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace AudioSwitcher.Hid
{

    internal static class NativeMethods
    {


        internal const short DIGCF_PRESENT = 0x2;
        internal const short DIGCF_DEVICEINTERFACE = 0x10;

        private static int HIDP_ERROR_CODES(int sev, ushort code)
        {
            return sev << 28 | 0x11 << 16 | code;
        }

        public static readonly int HIDP_STATUS_SUCCESS = HIDP_ERROR_CODES(0, 0);
        public static readonly int HIDP_STATUS_INVALID_PREPARSED_DATA = HIDP_ERROR_CODES(12, 1);

        [Flags]
        public enum EFileAttributes : uint
        {
            Readonly = 0x00000001,
            Hidden = 0x00000002,
            System = 0x00000004,
            Directory = 0x00000010,
            Archive = 0x00000020,
            Device = 0x00000040,
            Normal = 0x00000080,
            Temporary = 0x00000100,
            SparseFile = 0x00000200,
            ReparsePoint = 0x00000400,
            Compressed = 0x00000800,
            Offline = 0x00001000,
            NotContentIndexed = 0x00002000,
            Encrypted = 0x00004000,
            WriteThrough = 0x80000000,
            Overlapped = 0x40000000,
            NoBuffering = 0x20000000,
            RandomAccess = 0x10000000,
            SequentialScan = 0x08000000,
            DeleteOnClose = 0x04000000,
            BackupSemantics = 0x02000000,
            PosixSemantics = 0x01000000,
            OpenReparsePoint = 0x00200000,
            OpenNoRecall = 0x00100000,
            FirstPipeInstance = 0x00080000
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DeviceInterfaceData
        {
            internal int cbSize;
            internal System.Guid InterfaceClassGuid;
            internal int Flags;
            internal IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DeviceInfoData
        {
            internal int cbSize;
            internal Guid ClassGuid;
            internal int DevInst;
            internal IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct DeviceInterfaceDetailData
        {
            internal int Size;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            internal string DevicePath;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DeviceAttributes
        {
            public int Size;
            public short VendorId;
            public short ProductId;
            public short VersionNumber;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DeviceCapabilities
        {
            public ushort Usage;
            public ushort UsagePage;
            public ushort InputReportByteLength;
            public ushort OutputReportByteLength;
            public ushort FeatureReportByteLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public ushort[] Reserved;
            public ushort NumberLinkCollectionNodes;
            public ushort NumberInputButtonCaps;
            public ushort NumberInputValueCaps;
            public ushort NumberInputDataIndices;
            public ushort NumberOutputButtonCaps;
            public ushort NumberOutputValueCaps;
            public ushort NumberOutputDataIndices;
            public ushort NumberFeatureButtonCaps;
            public ushort NumberFeatureValueCaps;
            public ushort NumberFeatureDataIndices;
        }


        [DllImport("hid.dll")]
        public static extern void HidD_GetHidGuid(out Guid hidGuid);

        [DllImport("hid.dll")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool HidD_GetAttributes(SafeFileHandle handle, ref DeviceAttributes attributes);

        [DllImport("hid.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool HidD_GetManufacturerString(SafeFileHandle handle, StringBuilder buffer, int bufferLengthInBytes);

        [DllImport("hid.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool HidD_GetProductString(SafeFileHandle handle, StringBuilder buffer, int bufferLengthInBytes);

        [DllImport("hid.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool HidD_GetSerialNumberString(SafeFileHandle handle, StringBuilder buffer, int bufferLengthInBytes);

        [DllImport("hid.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool HidD_GetPreparsedData(SafeFileHandle handle, out IntPtr preparsed);

        [DllImport("hid.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool HidD_FreePreparsedData(IntPtr preparsed);

        [DllImport("hid.dll", CharSet = CharSet.Auto)]
        public static extern int HidP_GetCaps(IntPtr preparsed, out DeviceCapabilities caps);

        [DllImport("hid.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool HidD_GetFeature(SafeFileHandle handle, byte[] buffer, int bufferLength);

        [DllImport("hid.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool HidD_SetFeature(SafeFileHandle handle, byte[] buffer, int bufferLength);


        [DllImport("setupapi.dll")]
        internal static extern bool SetupDiEnumDeviceInfo(IntPtr deviceInfoSet, int memberIndex, ref DeviceInfoData deviceInfoData);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr notificationFilter, Int32 flags);

        [DllImport("setupapi.dll")]
        internal static extern int SetupDiCreateDeviceInfoList(ref Guid classGuid, int hwndParent);

        [DllImport("setupapi.dll")]
        internal static extern int SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

        [DllImport("setupapi.dll")]
        internal static extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, ref DeviceInfoData deviceInfoData, ref Guid interfaceClassGuid, int memberIndex, ref DeviceInterfaceData deviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, string enumerator, int hwndParent, int flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, EntryPoint = "SetupDiGetDeviceInterfaceDetail")]
        internal static extern bool SetupDiGetDeviceInterfaceDetailBuffer(IntPtr deviceInfoSet, ref DeviceInterfaceData deviceInterfaceData, IntPtr deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, IntPtr deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        internal static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref DeviceInterfaceData deviceInterfaceData, ref DeviceInterfaceDetailData deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, IntPtr deviceInfoData);

        [DllImport("user32.dll")]
        internal static extern bool UnregisterDeviceNotification(IntPtr handle);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess,
            [MarshalAs(UnmanagedType.U4)] FileShare fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] EFileAttributes flags,
            IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);
    }
}
