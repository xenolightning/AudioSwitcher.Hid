using System;
using System.IO;

namespace AudioSwitcher.Hid
{
    public class WriteableHidDevice : HidDevice, IWriteableHidDevice
    {

        public void SetFeature(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public void WriteReport(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public WriteableHidDevice(string path) : base(path)
        {
        }

        protected override FileAccess GetFileAccess()
        {
            return FileAccess.ReadWrite;
        }
    }
}
