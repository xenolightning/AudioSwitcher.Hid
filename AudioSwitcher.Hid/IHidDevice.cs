using System;

namespace AudioSwitcher.Hid
{
    public interface IHidDevice : IDisposable
    {

        string DevicePath { get; }

        string Manufacturer { get; }

        int MaxFeatureReportLength { get; }

        int MaxInputReportLength { get; }

        int MaxOutputReportLength { get; }

        int ProductId { get; }

        string ProductName { get; }

        int ProductVersion { get; }

        string SerialNumber { get; }

        int VendorId { get; }

        event Action<byte[]> ReportRead;

        event Action<byte[]> FeatureRead;

    }
}
