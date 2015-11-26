using System.Collections.Generic;

namespace AudioSwitcher.Hid.Logitech
{
    public static class LogitechDevices
    {

        public static IEnumerable<G933> GetG933Devices()
        {
            foreach (var hidDevice in HidDevices.Enumerate(Constants.LOGITECH_VENDOR_ID, Constants.LOGITECH_G933_PRODUCT_ID))
            {
                yield return new G933(hidDevice);
            }
        }

    }
}
