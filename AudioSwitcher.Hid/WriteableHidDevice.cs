using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

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
