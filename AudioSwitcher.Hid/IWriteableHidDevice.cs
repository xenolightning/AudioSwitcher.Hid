using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioSwitcher.Hid
{
    public interface IWriteableHidDevice : IHidDevice
    {

        void SetFeature(byte[] buffer);

        void WriteReport(byte[] buffer);
    }
}
