namespace AudioSwitcher.Hid
{
    public interface IWriteableHidDevice : IHidDevice
    {

        void SetFeature(byte[] buffer);

        void WriteReport(byte[] buffer);
    }
}
